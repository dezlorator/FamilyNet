﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using FamilyNetServer.Models.ViewModels;
using FamilyNetServer.Infrastructure;
using FamilyNetServer.Controllers;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class RepresentativesController : BaseController
    {
        #region Ctor

        public RepresentativesController(IUnitOfWorkAsync unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        #region Methods

        // GET: Representatives
        [AllowAnonymous]
        public async Task<IActionResult> Index(int id,
            PersonSearchModel searchModel)
        {
            IEnumerable<Representative> representatives = _unitOfWorkAsync.Representatives.GetAll();

            representatives = RepresentativeFilter.GetFiltered(representatives, searchModel);

            if (id == 0)
                return View(representatives);

            if (id > 0)
                representatives = representatives.Where(x => x.Orphanage.ID.Equals(id));

            return View(representatives);
        }

        // GET: Representatives/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var representative = await _unitOfWorkAsync.Representatives.GetById((int)id);

            if (representative == null)
                return NotFound();

            return View(representative);
        }

        [Authorize(Roles = "Admin, Representative")]
        // GET: Representatives/Create
        public async Task<IActionResult> Create()
        {
            await Check();

            List<Orphanage> orphanages = await _unitOfWorkAsync.Orphanages.GetAll()
                .OrderBy(o => o.Name).ToListAsync();

            ViewBag.Orphanages = new SelectList(orphanages, "ID", "Name");

            return View();
        }



        // POST: Representatives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Representative")]
        public async Task<IActionResult> Create([Bind("FullName,Birthday,Rating,Avatar,Orphanage")]
        Representative representative, int id, IFormFile file)
        {
            await ImageHelper.SetAvatar(representative, file, "wwwroot\\representatives");

            if (ModelState.IsValid)
            {
                var orphanage = await _unitOfWorkAsync.Orphanages.GetById(id);
                representative.Orphanage = orphanage;

                await _unitOfWorkAsync.Representatives.Create(representative);
                await _unitOfWorkAsync.Representatives.SaveChangesAsync();

                var user = await GetCurrentUserAsync();
                user.PersonID = representative.ID;
                user.PersonType = FamilyNetServer.Models.Identity.PersonType.Representative;
                await _unitOfWorkAsync.UserManager.UpdateAsync(user);

                return RedirectToAction(nameof(Index));
            }
            return View(representative);
        }

        // GET: Representatives/Edit/5
        [Authorize(Roles = "Admin, Representative")]
        public async Task<IActionResult> Edit(int? id)
        {

            List<Orphanage> orphanages = _unitOfWorkAsync.Orphanages.GetAll()
                .OrderBy(o => o.Name).ToList();
            ViewBag.Orphanages = new SelectList(orphanages, "ID", "Name");


            if (id == null)
                return NotFound();

            var check = CheckById((int)id).Result;
            var checkResult = check != null;
            if (checkResult)
            {
                return check;
            }



            var representative = await _unitOfWorkAsync.Representatives.GetById((int)id);

            if (representative == null)
                return NotFound();

            return View(representative);
        }

        // POST: Representatives/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Representative")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FullName,Birthday,Rating,Avatar,Orphanage")]
         Representative representative, int orphanageId, IFormFile file)
        {
            if (id != representative.ID)
                return NotFound();

            var check = CheckById((int)id).Result;
            var checkResult = check != null;
            if (checkResult)
            {
                return check;
            }

            await ImageHelper.SetAvatar(representative, file, "wwwroot\\representatives");

            if (ModelState.IsValid)
            {
                try
                {
                    var orphanage = await _unitOfWorkAsync.Orphanages.GetById(orphanageId);
                    representative.Orphanage = orphanage;

                    var representativeToEdit = await _unitOfWorkAsync.Representatives.GetById(representative.ID);
                    representativeToEdit.CopyState(representative);
                    _unitOfWorkAsync.Representatives.Update(representativeToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_unitOfWorkAsync.Representatives.Any(representative.ID))
                        return NotFound();
                    else
                        throw; //TODO: Loging
                }

                return RedirectToAction(nameof(Index));
            }

            return View(representative);
        }

        // GET: Representatives/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var representative = await _unitOfWorkAsync.Representatives.GetById((int)id);

            if (representative == null)
                return NotFound();

            return View(representative);
        }

        // POST: Representatives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var representative = await _unitOfWorkAsync.Representatives.GetById((int)id);
            await _unitOfWorkAsync.Representatives.Delete(representative.ID);
            _unitOfWorkAsync.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}