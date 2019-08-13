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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Infrastructure;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class OrphansController : BaseController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public OrphansController(IUnitOfWorkAsync unitOfWork, IHostingEnvironment environment) : base(unitOfWork)
        {
            _hostingEnvironment = environment;
        }       

        // GET: Orphans
        [AllowAnonymous]
        public async Task<IActionResult> Index(uint id)
        {
            var orphans = from o in _unitOfWorkAsync.Orphans.GetAll()
                        where (id == 0 || o.ID == id)
                        select o;

            return View(orphans);
        }

        // GET: Orphans/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orphan = await _unitOfWorkAsync.Orphans.GetById((int)id);
            if (orphan == null)
            {
                return NotFound();
            }

            return View(orphan);
        }

        // GET: Orphans/Create
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            ViewBag.ListOfOrphanages = _unitOfWorkAsync.Orphanages.GetAll();

            return View();
        }

        // POST: Orphans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("FullName,Address,Birthday,Orphanage,Avatar")] Orphan orphan, int id, IFormFile file)
        {
            await ImageHelper.SetAvatar(orphan, file, "wwwroot\\children");

            if (ModelState.IsValid)
            {
                var orphanage = await _unitOfWorkAsync.Orphanages.GetById(id);
                orphan.Orphanage = orphanage;
                
                await _unitOfWorkAsync.Orphans.Create(orphan);
                await _unitOfWorkAsync.Orphans.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(orphan);
        }

        // GET: Orphans/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Orphanage> orphanagesList = _unitOfWorkAsync.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;

            var orphan = await _unitOfWorkAsync.Orphans.GetById((int)id);
            
            if (orphan == null)
            {
                return NotFound();
            }

            return View(orphan);
        }

        // POST: Orphans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FullName,Birthday,Orphanage,Avatar")] Orphan orphan, int idOrphanage, IFormFile file)
        {
            if (id != orphan.ID)
            {
                return NotFound();
            }


            //if (file != null && file.Length > 0)
            //{
            //    var fileName = Path.GetRandomFileName();
            //    fileName = Path.ChangeExtension(fileName, ".jpg");
            //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\children", fileName);

            //    using (var fileSteam = new FileStream(filePath, FileMode.Create))
            //    {
            //        await file.CopyToAsync(fileSteam);
            //    }
            //    orphan.Avatar = fileName;
            //}

            await ImageHelper.SetAvatar(orphan, file, "wwwroot\\children");


            if (ModelState.IsValid)
            {
                try
                {
                    var orphanage = await _unitOfWorkAsync.Orphanages.GetById(idOrphanage);
                    orphan.Orphanage = orphanage;
                    var orphanToEdit = await _unitOfWorkAsync.Orphans.GetById(orphan.ID);
                    orphanToEdit.CopyState(orphan);
                    _unitOfWorkAsync.Orphans.Update(orphanToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_unitOfWorkAsync.Orphans.Any(orphan.ID))
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
            return View(orphan);
        }
       
        // GET: Orphans/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orphan = await _unitOfWorkAsync.Orphans.GetById((int)id);
            if (orphan == null)
            {
                return NotFound();
            }

            return View(orphan);
        }

        // POST: Orphans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orphan = await _unitOfWorkAsync.Orphans.GetById((int)id);
            if(orphan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            await _unitOfWorkAsync.Orphans.Delete((int)id);
            _unitOfWorkAsync.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Orphans/OrphansTable
        [Authorize(Roles = "Admin, Representative, Orphan")]
        public IActionResult OrphansTable()
        {
            var list = _unitOfWorkAsync.Orphans.GetAll().ToList();
            return View(list);
        }

    }
}
