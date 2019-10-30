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
using Microsoft.Extensions.Localization;

namespace FamilyNetServer.Controllers
{
    [Authorize]
    public class VolunteersController : BaseController
    {
        #region Fields

        private readonly IStringLocalizer<VolunteersController> _localizer;

        #endregion

        public VolunteersController(IUnitOfWork unitOfWork, IStringLocalizer<VolunteersController> localizer, IStringLocalizer<SharedResource> sharedLocalizer) : base(unitOfWork, sharedLocalizer)
        {
            _localizer = localizer;
        }

        // GET: Volunteers
        [AllowAnonymous]
        public async Task<IActionResult> Index(PersonSearchModel searchModel)
        {

            IEnumerable<Volunteer> volunteers = _unitOfWork.Volunteers.GetAll();

            volunteers = VolunteerFilter.GetFiltered(volunteers, searchModel);
            GetViewData();

            return View(volunteers);
        }

        // GET: Volunteers/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            GetViewData();

            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _unitOfWork.Volunteers.GetById((int)id);
            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // GET: Volunteers/Create
        [Authorize(Roles = "Admin, Volunteer")]
        public IActionResult Create()
        {
            Check();
            GetViewData();

            List<Orphanage> orphanagesList = new List<Orphanage>();
            orphanagesList = _unitOfWork.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;

            GetViewData();
            return View();
        }

        // POST: Volunteers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Volunteer")]
        public async Task<IActionResult> Create([Bind("FullName, Address, Birthday, Contacts")] Volunteer volunteer)
        {


            if (ModelState.IsValid)
            {
                await _unitOfWork.Volunteers.Create(volunteer);
                await _unitOfWork.Volunteers.SaveChangesAsync();

                var user = await GetCurrentUserAsync();
                user.PersonID = volunteer.ID;
                user.PersonType = Models.Identity.PersonType.Volunteer;
                await _unitOfWork.UserManager.UpdateAsync(user);
                GetViewData();

                return RedirectToAction(nameof(Index));
            }
            GetViewData();

            return View(volunteer);
        }

        // GET: Volunteers/Edit/5
        [Authorize(Roles = "Admin, Volunteer")]
        public async Task<IActionResult> Edit(int? id)
        {
            GetViewData();

            if (id == null)
            {
                return NotFound();
            }

            var check = CheckById((int)id).Result;
            var checkResult = check != null;
            if (checkResult)
            {
                return check;
            }

            var volunteer = await _unitOfWork.Volunteers.GetById((int)id);
            if (volunteer == null)
            {
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
        public async Task<IActionResult> Edit(int id, [Bind("ID, FullName, Address, Birthday, Contacts, Rating")] Volunteer volunteer)
        {
            GetViewData();

            if (id != volunteer.ID)
            {
                return NotFound();
            }

            var check = CheckById((int)id).Result;
            var checkResult = check != null;
            if (checkResult)
            {
                return check;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var volunteerToEdit = await _unitOfWork.Volunteers.GetById(id);
                    volunteerToEdit.CopyState(volunteer);
                    _unitOfWork.Volunteers.Update(volunteerToEdit);
                    _unitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_unitOfWork.Volunteers.Any(volunteer.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(volunteer);
        }

        // GET: Volunteers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _unitOfWork.Volunteers
                .GetById((int)id);
            if (volunteer == null)
            {
                return NotFound();
            }
            GetViewData();

            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteer = await _unitOfWork.Volunteers.GetById(id);
            await _unitOfWork.Volunteers.Delete(volunteer.ID);
            _unitOfWork.SaveChangesAsync();
            GetViewData();

            return RedirectToAction(nameof(Index));
        }

        #region PrivateHelpers
        private void GetViewData()
        {
            ViewData["CreateVolonteer"] = _localizer["CreateVolonteer"];
            ViewData["OurVolonteers"] = _localizer["OurVolonteers"];
        }

        #endregion
    }
}
