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

namespace FamilyNet.Controllers
{
    public class VolunteersController : BaseController
    {
        public VolunteersController(IUnitOfWorkAsync unitOfWork)
            : base(unitOfWork) { }

        // GET: Volunteer
        public async Task<IActionResult> Index()
        {
            var list = await _unitOfWorkAsync.Volunteers.GetAll().ToListAsync();
            return View(list);
        }

        // GET: Volunteer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = await _unitOfWorkAsync.CharityMakers.GetById((int)id);

            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // GET: Volunteers/Create
        public IActionResult Create() => View();

        // POST: Volunteers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Birthday,Rating")] Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWorkAsync.Volunteers.Create(volunteer);
                await _unitOfWorkAsync.Volunteers.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        // GET: Volunteers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var volunteer = await _unitOfWorkAsync.Volunteers.GetById((int)id);

            if (volunteer == null)
                return NotFound();

            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Birthday,Rating")] Volunteer volunteer)
        {
            if (id != volunteer.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    //var volunteerToEdit = await _unitOfWorkAsync.Volunteers.GetById(volunteer.ID);
                    //Volunteer.CopyState(volunteerToEdit, volunteer);
                    //_unitOfWorkAsync.Volunteers.Update(volunteerToEdit);
                    //_unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolunteerExists(volunteer.ID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        // GET: Volunteer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var volunteer = await _unitOfWorkAsync.Volunteers.GetById((int)id);
            if (volunteer == null)
                return NotFound();

            return View(volunteer);
        }
        // POST: Orphans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteer = await _unitOfWorkAsync.Volunteers.GetById((int)id);
            await _unitOfWorkAsync.Volunteers.Delete((int)id);
            _unitOfWorkAsync.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private bool VolunteerExists(int id)
         => _unitOfWorkAsync.Volunteers.GetById(id) != null;
    }
}