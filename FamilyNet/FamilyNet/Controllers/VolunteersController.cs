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

namespace FamilyNet.Controllers
{
    /// <summary>
    /// The controller accepts and processes requests about Volunteers
    /// </summary>
    [Authorize]
    public class VolunteersController : BaseController
    {
        #region Fields

        private readonly IStringLocalizer<VolunteersController> _localizer;

        #endregion

        public VolunteersController(IUnitOfWorkAsync unitOfWork, IStringLocalizer<VolunteersController> localizer, IStringLocalizer<SharedResource> sharedLocalizer) : base(unitOfWork, sharedLocalizer)
        {
            _localizer = localizer;
        }

        // GET: Volunteers
        /// <summary>
        /// Method provides list of Volunteers
        /// </summary>
        /// <param name="searchModel">parameters for filter</param>
        /// <returns>View with list of Volunteers</returns>
        [AllowAnonymous]
        public async Task<IActionResult> Index(PersonSearchModel searchModel)
        {

            IEnumerable<Volunteer> volunteers = _unitOfWorkAsync.Volunteers.GetAll();

            volunteers = VolunteerFilter.GetFiltered(volunteers, searchModel);
            GetViewData();

            return View(volunteers);
        }

        // GET: Volunteers/Details/5
        /// <summary>
        /// Method provides information about volunteer by id
        /// </summary>
        /// <param name="id">Volunteer's identifier</param>
        /// <returns>View with details about Volunteer or NotFound page</returns>
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            GetViewData();

            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _unitOfWorkAsync.Volunteers.GetById((int)id);
            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // GET: Volunteers/Create
        /// <summary>
        /// Method provides a view for adding new Volunteer into database
        /// </summary>
        /// <returns>View with input form</returns>
        [Authorize(Roles = "Admin, Volunteer")]
        public IActionResult Create()
        {
            Check();
            GetViewData();

            List<Orphanage> orphanagesList = new List<Orphanage>();
            orphanagesList = _unitOfWorkAsync.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;

            GetViewData();
            return View();
        }

        // POST: Volunteers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Method validates and adds new Volunteer into database
        /// </summary>
        /// <param name="volunteer">adding entity</param>
        /// <returns>Redirect to Index if model is valid or return to Create page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Volunteer")]
        public async Task<IActionResult> Create([Bind("FullName, Address, Birthday, Contacts")] Volunteer volunteer)
        {


            if (ModelState.IsValid)
            {
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
        /// <summary>
        ///  Method provides view for editing Volunteer
        /// </summary>
        /// <param name="id">Volunteer's identifier</param>
        /// <returns>View with editing entity</returns>
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

            var volunteer = await _unitOfWorkAsync.Volunteers.GetById((int)id);
            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Method validates and updates Volunteer in database
        /// </summary>
        /// <param name="id">Volunteer's identifier</param>
        /// <param name="volunteer">Volunteer's details by id</param>
        /// <returns>Redirect to index if model is valid or return to Edit page</returns>
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
                    var volunteerToEdit = await _unitOfWorkAsync.Volunteers.GetById(id);
                    volunteerToEdit.CopyState(volunteer);
                    _unitOfWorkAsync.Volunteers.Update(volunteerToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_unitOfWorkAsync.Volunteers.Any(volunteer.ID))
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
        /// <summary>
        /// Method provides page for deleting Volunteer by id from database
        /// </summary>
        /// <param name="id">Volunteer's identifier</param>
        /// <returns>Delete view if model is valid or redirect to NotFound page</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _unitOfWorkAsync.Volunteers
                .GetById((int)id);
            if (volunteer == null)
            {
                return NotFound();
            }
            GetViewData();

            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        /// <summary>
        /// Method deletes Volunteer by id from database
        /// </summary>
        /// <param name="id">Volunteer's identifier</param>
        /// <returns>Redirect to Index page</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteer = await _unitOfWorkAsync.Volunteers.GetById(id);
            await _unitOfWorkAsync.Volunteers.Delete(volunteer.ID);
            _unitOfWorkAsync.SaveChangesAsync();
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
