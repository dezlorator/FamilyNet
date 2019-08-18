using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FamilyNet.Models;
using FamilyNet.Models.EntityFramework;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Models.ViewModels;
using FamilyNet.Infrastructure;
using Microsoft.Extensions.Localization;

namespace FamilyNet.Controllers {
    [Authorize]
    public class VolunteersController : BaseController {
        #region Fields

        private readonly IStringLocalizer<VolunteersController> _localizer;

        #endregion

        public VolunteersController(IUnitOfWorkAsync unitOfWork, IStringLocalizer<VolunteersController> localizer, IStringLocalizer<SharedResource> sharedLocalizer) : base(unitOfWork, sharedLocalizer) {
            _localizer = localizer;
        }

        // GET: Volunteers
        [AllowAnonymous]
        public async Task<IActionResult> Index(PersonSearchModel searchModel) {

            IEnumerable<Volunteer> volunteers = _unitOfWorkAsync.Volunteers.GetAll();

            volunteers = VolunteerFilter.GetFiltered(volunteers, searchModel);
            GetViewData();

            return View(volunteers);
        }

        // GET: Volunteers/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id) 
        {
            if (id == null) {
                return NotFound();
            }

            var volunteer = await _unitOfWorkAsync.Volunteers.GetById((int)id);
            if (volunteer == null) {
                return NotFound();
            }
         
            return View(volunteer);
        }

        // GET: Volunteers/Create
        [Authorize(Roles = "Admin, Volunteer")]
        public IActionResult Create() {
            var access = CheckAccess().Result;
            

            GetViewData();

            List<Orphanage> orphanagesList = new List<Orphanage>();
            orphanagesList = _unitOfWorkAsync.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;

            GetViewData();
            return access == null ? access : View();
        }

        // POST: Volunteers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Volunteer")]
        public async Task<IActionResult> Create([Bind("FullName, Address, Birthday, Contacts")] Volunteer volunteer) {


            if (ModelState.IsValid) {
                await _unitOfWorkAsync.Volunteers.Create(volunteer);
                await _unitOfWorkAsync.Volunteers.SaveChangesAsync();

                var user = await GetCurrentUserAsync();
                user.PersonID = volunteer.ID;
                user.PersonType = Models.Identity.PersonType.Volunteer;
                await _unitOfWorkAsync.UserManager.UpdateAsync(user);
                GetViewData();

                return RedirectToAction(nameof(Index));
            }
            GetViewData();

            return View(volunteer);
        }

        // GET: Volunteers/Edit/5
        [Authorize(Roles = "Admin, Volunteer")]
        public async Task<IActionResult> Edit(int? id) {
            GetViewData();

            if (id == null) {
                return NotFound();
            }

            var check = CheckAccess((int)id).Result;
            var checkResult = check != null;
            if (checkResult) {
                return check;
            }

            var volunteer = await _unitOfWorkAsync.Volunteers.GetById((int)id);
            if (volunteer == null) {
                return NotFound();
            }

            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Volunteer")]
        public async Task<IActionResult> Edit(int id, [Bind("ID, FullName, Address, Birthday, Contacts, Rating")] Volunteer volunteer) {
            GetViewData();

            if (id != volunteer.ID) {
                return NotFound();
            }

            var check = CheckAccess((int)id).Result;
            var checkResult = check != null;
            if (checkResult) {
                return check;
            }

            if (ModelState.IsValid) {
                try {
                    var volunteerToEdit = await _unitOfWorkAsync.Volunteers.GetById(id);
                    volunteerToEdit.CopyState(volunteer);
                    _unitOfWorkAsync.Volunteers.Update(volunteerToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!_unitOfWorkAsync.Volunteers.Any(volunteer.ID)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(volunteer);
        }

        // GET: Volunteers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var volunteer = await _unitOfWorkAsync.Volunteers
                .GetById((int)id);
            if (volunteer == null) {
                return NotFound();
            }

            ViewData["AreYouSure"] = _localizer["AreYouSure"];

            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var volunteer = await _unitOfWorkAsync.Volunteers.GetById(id);
            await _unitOfWorkAsync.Volunteers.Delete(volunteer.ID);
            _unitOfWorkAsync.SaveChangesAsync();
            GetViewData();

            return RedirectToAction(nameof(Index));
        }

        #region PrivateHelpers
        private void GetViewData() {
            ViewData["CreateVolonteer"] = _localizer["CreateVolonteer"];
            ViewData["OurVolonteers"] = _localizer["OurVolonteers"];
        }

        #endregion
    }
}
