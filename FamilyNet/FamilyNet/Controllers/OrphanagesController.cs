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
using FamilyNet.Models.Filters;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace FamilyNet.Controllers
{
    public class OrphanagesController : BaseController
    {
        private OrphanageSearchModel _searchModel;
        private readonly IHostingEnvironment _hostingEnvironment;
        public OrphanagesController(IUnitOfWorkAsync unitOfWork, IHostingEnvironment environment) : base(unitOfWork)
        {
            _hostingEnvironment = environment;
        }

        private bool IsContain(Address addr)
        {
            foreach (var word in _searchModel.AddressString.Split())
            {
                if (addr.Street.ToUpper().Contains(word.ToUpper())
                || addr.City.ToUpper().Contains(word.ToUpper())
                || addr.Region.ToUpper().Contains(word.ToUpper())
                || addr.Country.ToUpper().Contains(word.ToUpper()))
                {
                    return true;
                }
            }
            return false;
        }
        // GET: Orphanages
        public async Task<IActionResult> Index(OrphanageSearchModel searchModel, SortStateOrphanages sortOrder = SortStateOrphanages.NameAsc)
        {
            //List<Orphanage> orphanagS = new List<Orphanage>()
            //{
            //    new Orphanage() {Name="", Adress = { new Address() { } } }
            //};

            IQueryable<Orphanage> orphanages = _unitOfWorkAsync.Orphanages.GetAll();
           
            if (searchModel != null)
            {
                _searchModel = searchModel;
                if (!string.IsNullOrEmpty(searchModel.NameString))
                    orphanages = orphanages.Where(x => x.Name.Contains(searchModel.NameString));
                if (!string.IsNullOrEmpty(searchModel.AddressString))
                    orphanages = orphanages.Where( x => IsContain(x.Adress));
                    
                if (searchModel.RatingNumber > 0)
                    orphanages = orphanages.Where(x => x.Rating == searchModel.RatingNumber);
            }

            ViewData["NameSort"] = sortOrder == SortStateOrphanages.NameAsc ? SortStateOrphanages.NameDesc : SortStateOrphanages.NameAsc;
            ViewData["AddressSort"] = sortOrder == SortStateOrphanages.AddressAsc ? SortStateOrphanages.AddressDesc : SortStateOrphanages.AddressAsc;
            ViewData["RatingSort"] = sortOrder == SortStateOrphanages.RatingAsc ? SortStateOrphanages.RatingDesc : SortStateOrphanages.RatingAsc;

            switch (sortOrder)
            {
                case SortStateOrphanages.NameDesc:
                    orphanages = orphanages.OrderByDescending(s => s.Name);
                    break;
                case SortStateOrphanages.AddressAsc:
                    orphanages = orphanages.OrderBy(s => s.Adress.Country).ThenBy(s => s.Adress.Region).ThenBy(s => s.Adress.City).ThenBy(s => s.Adress.Street);
                    break;
                case SortStateOrphanages.AddressDesc:
                    orphanages = orphanages.OrderByDescending(s => s.Adress.Country).ThenByDescending(s => s.Adress.Region).ThenByDescending(s => s.Adress.City).ThenByDescending(s => s.Adress.Street);
                    break;
                case SortStateOrphanages.RatingAsc:
                    orphanages = orphanages.OrderBy(s => s.Rating);
                    break;
                case SortStateOrphanages.RatingDesc:
                    orphanages = orphanages.OrderByDescending(s => s.Rating);
                    break;
                default:
                    orphanages = orphanages.OrderBy(s => s.Name);
                    break;
            }
            
            return View(await orphanages.ToListAsync());
        }

        // GET: Orphanages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);
            if (orphanage == null)
                return NotFound();

            return View(orphanage);
        }

        // GET: Orphanages/Create
        public IActionResult Create() => View();

        // POST: Orphanages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Adress,Rating,Avatar")] Orphanage orphanage, IFormFile file)
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
                orphanage.Avatar = fileName;
            }
            if (ModelState.IsValid)
            {
                bool rand = DateTime.Now.Ticks % 2 == 0;
                List<DonationItemType> test = new List<DonationItemType>();
                test.Add(new DonationItemType()
                {
                    Name = rand ? "одежда" : "игрушки"
                });
                orphanage.Needs = new List<DonationItem>();
                orphanage.Needs.Add(new DonationItem()
                {
                    Description = "тест",
                    Name = "Имя",
                    Price = 2,
                    DonationItemTypes = test,
                });
                await _unitOfWorkAsync.Orphanages.Create(orphanage);
                await _unitOfWorkAsync.Orphanages.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(orphanage);
        }
        
        // GET: Orphanages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);
            if (orphanage == null)
                return NotFound();

            return View(orphanage);
        }

        // POST: Orphanages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Adress,Rating,Avatar")] Orphanage orphanage, IFormFile file)
        {
            if (id != orphanage.ID)
                return NotFound();

            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetRandomFileName();
                fileName = Path.ChangeExtension(fileName, ".jpg");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\avatars", fileName);
                using (var fileSteam = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileSteam);
                }
                orphanage.Avatar = fileName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //in ef to change the object you need to track it out of context
                    var orphanageToEdit = await _unitOfWorkAsync.Orphanages.GetById(orphanage.ID);
                    //copying the state with NOT CHANGING REFERENCES
                    Orphanage.CopyState(orphanageToEdit, orphanage);
                    _unitOfWorkAsync.Orphanages.Update(orphanageToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrphanageExists(orphanage.ID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(orphanage);
        }

        // GET: Orphanages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);
            if (orphanage == null)
                return NotFound();

            return View(orphanage);
        }

        // POST: Orphanages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);
            await _unitOfWorkAsync.Orphanages.Delete(orphanage.ID);
            _unitOfWorkAsync.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool OrphanageExists(int id) =>
            _unitOfWorkAsync.Orphanages.GetById(id) != null;

        public async Task<IActionResult> SearchByTypeHelp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SearchResult(string typeHelp)
        {
            ViewData["TypeHelp"] = typeHelp;
            var list = _unitOfWorkAsync.Orphanages.Get(
                orp => orp.Needs.Where(
                    donat => donat.DonationItemTypes.Where(
                        donatitem => donatitem.Name.ToLower().Contains(typeHelp.ToLower())).ToList().Count > 0).ToList().Count > 0);
            return View("SearchResult", list);
        }
    }
}
