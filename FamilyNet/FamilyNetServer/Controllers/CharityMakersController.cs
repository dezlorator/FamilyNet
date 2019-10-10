using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FamilyNetServer.Models;
using FamilyNetServer.Models.EntityFramework;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using FamilyNetServer.Models.ViewModels;
using FamilyNetServer.Infrastructure;

namespace FamilyNetServer.Controllers
{
    [Authorize]
    public class CharityMakersController : BaseController
    {
        public CharityMakersController(IUnitOfWorkAsync unitOfWork) : base (unitOfWork)
        {

        }

        // GET: CharityMakers
        [AllowAnonymous]
        public async Task<IActionResult> Index(PersonSearchModel searchModel)
        {
            IEnumerable<CharityMaker> charityMakers =  _unitOfWorkAsync.CharityMakers.GetAll();

            charityMakers = CharityMakerFilter.GetFiltered(charityMakers, searchModel);

            return View(charityMakers); 
        }

        // GET: CharityMakers/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var charityMaker = await _unitOfWorkAsync.CharityMakers.GetById((int)id);

            if (charityMaker == null)
            {
                return NotFound();
            }

            return View(charityMaker);
        }

        // GET: CharityMakers/Create
        [Authorize(Roles = "Admin, CharityMaker")]
        public IActionResult Create()
        {
            Check();
            return View();
        }

        // POST: CharityMakers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, CharityMaker")]
        public async Task<IActionResult> Create([Bind("ID,FullName,Address,Birthday,Contacts,Rating")] CharityMaker charityMaker)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWorkAsync.CharityMakers.Create(charityMaker);
                await _unitOfWorkAsync.CharityMakers.SaveChangesAsync();

                var user = await GetCurrentUserAsync();
                user.PersonID = charityMaker.ID;
                user.PersonType = Models.Identity.PersonType.CharityMaker;
                await _unitOfWorkAsync.UserManager.UpdateAsync(user);

                return RedirectToAction(nameof(Index));
            }
            return View(charityMaker);
        }

        // GET: CharityMakers/Edit/5
        [Authorize(Roles = "Admin, CharityMaker")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var check = CheckById((int)id).Result;
            var checkResult = check != null;
            if(checkResult)
            {
                return check;
            }

            var charityMaker = await _unitOfWorkAsync.CharityMakers.GetById((int)id);

            if (charityMaker == null)
            {
                return NotFound();
            }

            return View(charityMaker);
        }

        // POST: CharityMakers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, CharityMaker")]
        public async Task<IActionResult> Edit(int id,
            [Bind("ID,FullName,Address,Birthday,Contacts,Rating")] CharityMaker charityMaker)
        {
            var check = CheckById((int)id).Result;
            var checkResult = check != null;
            if (checkResult)
            {
                return check;
            }

            if (id != charityMaker.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    CharityMaker charityMakerToEdit = await _unitOfWorkAsync.CharityMakers.GetById(id);
                    charityMakerToEdit.CopyState(charityMaker);
                    _unitOfWorkAsync.CharityMakers.Update(charityMakerToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_unitOfWorkAsync.CharityMakers.Any(charityMaker.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // TODO : logging
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(charityMaker);
        }

        // GET: CharityMakers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CharityMaker charityMaker = await _unitOfWorkAsync.CharityMakers
                .GetById((int)id);

            if (charityMaker == null)
            {
                return NotFound();
            }

            return View(charityMaker);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!_unitOfWorkAsync.CharityMakers.Any(id))
            {
                await _unitOfWorkAsync.CharityMakers.Delete(id);
                _unitOfWorkAsync.SaveChangesAsync();
            }          

            return RedirectToAction(nameof(Index));
        }

        private bool HasCharityMaker(int id)
        {
            return _unitOfWorkAsync.CharityMakers.GetById(id) != null;
        }

        // GET: CharityMakers/Table
        [Authorize(Roles = "Admin")]
        public IActionResult Table()
        {
            var list = _unitOfWorkAsync.CharityMakers.GetAll().ToList();
            return View(list);
        }

    }
}
