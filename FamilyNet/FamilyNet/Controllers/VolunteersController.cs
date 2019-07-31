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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class VolunteersController : BaseController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public VolunteersController(IUnitOfWorkAsync unitOfWork, IHostingEnvironment environment) : base(unitOfWork)
        {
            _hostingEnvironment = environment;
        }
        //public VolunteersController(IUnitOfWorkAsync unitOfWork) : base(unitOfWork)
        //{ }

        // GET: Volunteers
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {

            var list = _unitOfWorkAsync.Volunteers.GetAll().ToList();
            return View(list);
        }

        // GET: Volunteers/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            List<Orphanage> orphanagesList = _unitOfWorkAsync.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;

            return View();
        }

        // POST: Volunteers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("FullName, Address, Birthday, Rating, Avatar")] Volunteer volunteer, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetRandomFileName();
                fileName = Path.ChangeExtension(fileName, ".jpg");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\avatars", fileName);
                using (var fileSteam = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileSteam);
                }
                volunteer.Avatar = fileName;
            }

            if (ModelState.IsValid)
            {
                await _unitOfWorkAsync.Volunteers.Create(volunteer);
                await _unitOfWorkAsync.Volunteers.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        // GET: Volunteers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
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

        // POST: Volunteers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, 
            [Bind("ID, FullName, Address, Birthday, Rating, Avatar")] Volunteer volunteer, IFormFile file)
        {
            if (id != volunteer.ID)
            {
                return NotFound();
            }

            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetRandomFileName();
                fileName = Path.ChangeExtension(fileName, ".jpg");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\avatars", fileName);
                using (var fileSteam = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileSteam);
                }
                volunteer.Avatar = fileName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var volunteerToEdit = await _unitOfWorkAsync.Volunteers.GetById(volunteer.ID);
                    Volunteer.CopyState(volunteerToEdit, volunteer);
                    _unitOfWorkAsync.Volunteers.Update(volunteerToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolunteerExists(volunteer.ID))
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

            var volunteer = await _unitOfWorkAsync.Volunteers
                .GetById((int)id);
            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var volunteer = await _unitOfWorkAsync.Volunteers.GetById(id);
            await _unitOfWorkAsync.Volunteers.Delete(volunteer.ID);
            _unitOfWorkAsync.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VolunteerExists(int id)
        {
            return _unitOfWorkAsync.Volunteers.GetById(id) != null;
        }
    }
}
