﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class DonationsController : BaseController
    {
        public DonationsController(IUnitOfWorkAsync unitOfWork) : base(unitOfWork)
        {

        }

        // GET: DonationsTable
        [AllowAnonymous]
        public IActionResult DonationsTable()
        {
            return View(_unitOfWorkAsync.Donations.GetAll());
        }

        // GET: CategoriesTable
        [AllowAnonymous]
        public IActionResult CategoriesTable()
        {
            return View(_unitOfWorkAsync.BaseItemTypes.GetAll());
        }

        #region CreateCategory

        // GET: DonationsTable/CreateCategory
        [Authorize(Roles = "Admin,CharityMaker,Volunteer,Representative")]
        public IActionResult CreateCategory()
        {
            return View();
        }

        // POST: Donations/CreateCategory
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,CharityMaker,Volunteer")]
        public async Task<IActionResult> CreateCategory([Bind("ID,Name")] DonationItemType request)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWorkAsync.BaseItemTypes.Create(request);
                await _unitOfWorkAsync.BaseItemTypes.SaveChangesAsync();
                return RedirectToAction(nameof(CategoriesTable));
            }
            return View(request);
        }

        #endregion

        #region DeleteCategory

        // GET: CategoriesTable/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BaseItemType category = await _unitOfWorkAsync.BaseItemTypes
                .GetById((int)id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: CategoriesTable/Delete/5
        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            var category = await _unitOfWorkAsync.BaseItemTypes.GetById((int)id);
            if (category == null)
            {
                return RedirectToAction(nameof(CategoriesTable));
            }
            await _unitOfWorkAsync.BaseItemTypes.Delete((int)id);
            _unitOfWorkAsync.SaveChangesAsync();

            return RedirectToAction(nameof(CategoriesTable));
        }

        #endregion

        #region CreateDonation

        // GET: DonationsTable/CreateRequest
        [Authorize(Roles = "Admin,CharityMaker,Volunteer,Representative")]
        public IActionResult CreateDonation()
        {
            ViewBag.ListOfOrphanages = _unitOfWorkAsync.Orphanages.GetAll();
            ViewBag.ListOfBaseItemTypes = _unitOfWorkAsync.BaseItemTypes.GetAll();
            return View();
        }

        // POST: DonationsTable/CreateRequest
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,CharityMaker,Volunteer")]
        public async Task<IActionResult> CreateDonation([Bind("ID,DonationItem,Orphanage")] Donation request, int idOrphanage, int idDonationItem)
        {
            if (ModelState.IsValid)
            {
                request.OrphanageID = idOrphanage;
                
                request.IsRequest = true;

                await _unitOfWorkAsync.Donations.Create(request);
                await _unitOfWorkAsync.Donations.SaveChangesAsync();

                _unitOfWorkAsync.TypeBaseItems.Add(new TypeBaseItem() { ItemID = request.DonationItem.ID, TypeID = idDonationItem  });

                _unitOfWorkAsync.SaveChangesAsync();

                return RedirectToAction(nameof(DonationsTable));
            }
            return View(request);
        }

        #endregion      

        #region EditDonation

        // GET: Donations/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditDonation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            ViewBag.ListOfOrphanages = _unitOfWorkAsync.Orphanages.GetAll();
            ViewBag.ListOfBaseItemTypes = _unitOfWorkAsync.BaseItemTypes.GetAll();
            var donation = await _unitOfWorkAsync.Donations.GetById((int)id);

            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }

        // POST: Donations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditDonation(int id,
            [Bind("ID,DonationItem")] Donation donation)
        {
            if (id != donation.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Donation donationToEdit = await _unitOfWorkAsync.Donations.GetById(id);
                    donationToEdit.CopyState(donation);
                    _unitOfWorkAsync.Donations.Update(donationToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonationExists(donation.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // TODO : logging
                    }
                }
                return RedirectToAction(nameof(DonationsTable));
            }
            return View(donation);
        }

        #endregion

        #region DeleteDonation

        // GET: DonationsTable/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDonation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Donation donation = await _unitOfWorkAsync.Donations
                .GetById((int)id);

            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }

        // POST: DonationsTable/Delete/5
        [HttpPost, ActionName("DeleteDonation")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDonationConfirmed(int id)
        {
            var donation = await _unitOfWorkAsync.Donations.GetById((int)id);
            if (donation == null)
            {
                return RedirectToAction(nameof(DonationsTable));
            }
            await _unitOfWorkAsync.Donations.Delete((int)id);
            _unitOfWorkAsync.SaveChangesAsync();

            return RedirectToAction(nameof(DonationsTable));
        }

        #endregion

        #region DetailsDonation

        // GET: Donations/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var donation = await _unitOfWorkAsync.Donations.GetById((int)id);
            if (donation == null)
            {
                return NotFound();
            }

            return View(donation);
        }

        #endregion

        #region DetailsCategory
        public async Task<IActionResult> DetailsCategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _unitOfWorkAsync.BaseItemTypes.GetById((int)id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        #endregion

        private bool DonationExists(int id)
        {
            return _unitOfWorkAsync.Donations.GetById(id) != null;
        }

        private bool CategoryExists(int id)
        {
            return _unitOfWorkAsync.BaseItemTypes.GetById(id) != null;
        }

    }
}