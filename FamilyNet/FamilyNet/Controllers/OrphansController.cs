﻿using System;
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
    public class OrphansController : BaseController
    {
        public OrphansController(IUnitOfWorkAsync unitOfWork) : base(unitOfWork) { }

        // GET: Orphans
        public async Task<IActionResult> Index(int id)
        {
            var list = _unitOfWorkAsync.Orphans.GetAll().ToList();
            if (id == 0)
                return View(list);

            if (id >0)
                list = list.Where(x => x.Orphanage.ID.Equals(id)).ToList();

            return View(list);
        }

        // GET: Orphans/Details/5
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
        public IActionResult Create()
        {
            List<Orphanage> orphanagesList = new List<Orphanage>();
            orphanagesList = _unitOfWorkAsync.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;

            return View();
        }

        // POST: Orphans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Address,Birthday,Contacts,Orphanage")] Orphan orphan)
        {
            //orphan.Orphanage.Name = _unitOfWorkAsync.Orphanages.GetById(orphan.Orphanage.ID).Result.Name;
            if (ModelState.IsValid)
            {
                var orphanageList = _unitOfWorkAsync.Orphanages.Get(orph => orph.ID == orph.ID).ToList();
                orphan.Orphanage = orphanageList[0];
                await _unitOfWorkAsync.Orphans.Create(orphan);
                await _unitOfWorkAsync.Orphans.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(orphan);
        }

        // GET: Orphans/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: Orphans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FullName,Birthday,Contacts")] Orphan orphan)
        {
            if (id != orphan.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var orphanToEdit = await _unitOfWorkAsync.Orphans.GetById(orphan.ID);
                    Orphan.CopyState(orphanToEdit, orphan);
                    _unitOfWorkAsync.Orphans.Update(orphanToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrphanExists(orphan.ID))
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

        private bool OrphanExists(int id)
        {
            return _unitOfWorkAsync.Orphans.GetById(id) != null;
        }
    }
}
