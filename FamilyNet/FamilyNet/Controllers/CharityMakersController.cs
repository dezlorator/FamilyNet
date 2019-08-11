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

namespace FamilyNet.Controllers
{
    [Authorize]
    public class CharityMakersController : BaseController
    {
        public CharityMakersController(IUnitOfWorkAsync unitOfWork) : base (unitOfWork)
        {

        }

        // GET: CharityMakers
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_unitOfWorkAsync.CharityMakers.GetAll());
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CharityMakers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ID,FullName,Address,Birthday,Contacts,Rating,Avatar")] CharityMaker charityMaker, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetRandomFileName();
                fileName = Path.ChangeExtension(fileName, ".jpg");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\children", fileName);
                using (var fileSteam = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileSteam);
                }
                charityMaker.Avatar = fileName;
            }

            if (ModelState.IsValid)
            {
                await _unitOfWorkAsync.CharityMakers.Create(charityMaker);
                await _unitOfWorkAsync.CharityMakers.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(charityMaker);
        }

        // GET: CharityMakers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
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

        // POST: CharityMakers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id,
            [Bind("ID,FullName,Address,Birthday,Contacts,Rating,Avatar")] CharityMaker charityMaker, IFormFile file)
        {
            if (id != charityMaker.ID)
            {
                return NotFound();
            }

            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetRandomFileName();
                fileName = Path.ChangeExtension(fileName, ".jpg");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\children", fileName);
                using (var fileSteam = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileSteam);
                }
                charityMaker.Avatar = fileName;
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
                    if (!HasCharityMaker(charityMaker.ID))
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
            if (HasCharityMaker(id))
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
