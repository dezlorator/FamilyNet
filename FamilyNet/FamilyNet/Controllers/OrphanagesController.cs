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
    public class OrphanagesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public OrphanagesController(IUnitOfWorkAsync unitOfWork, ApplicationDbContext con) : base(unitOfWork)
        {
            _context = con;
        }

        // GET: Orphanages
        public async Task<IActionResult> Index()
        {
            //await _unitOfWorkAsync.Orphanages.Create(new Orphanage()
            //{
            //    Name = "name33",
            //    Adress = new Adress() { City = "test3", Country = "test3", House = "test3", Region = "test3", Street = "test3" },
            //    Rating = 5,
            //    Avatar = "123123123"
            //});
            //_unitOfWorkAsync.SaveChangesAsync();
            var list =  _unitOfWorkAsync.Orphanages.GetAll()/*.Include(a => a.Adress)*/.ToList();
            return View(list);
            //return View(list);
        }

        // GET: Orphanages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            //var orphanage = await _context.Orphanages.Include(a => a.Adress)
            //     .FirstOrDefaultAsync(m => m.ID == id);
            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);
            if (orphanage == null)
                return NotFound();

            return View(orphanage);
        }

        // GET: Orphanages/Create
        public IActionResult Create() => View();

        // POST: Orphanages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(/*[Bind("ID,Name,Rating,Avatar")]*/ Orphanage orphanage)
        {
            if (ModelState.IsValid)
            {
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
            //var orphanage = await _context.Orphanages.Include(a => a.Adress)
            //     .FirstOrDefaultAsync(m => m.ID == id);
            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);
            if (orphanage == null)
                return NotFound();

            return View(orphanage);
        }

        // POST: Orphanages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Adress,Rating,Avatar")] Orphanage orphanage)
        {
            if (id != orphanage.ID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    //_unitOfWorkAsync.Orphanages.Update(orphanage);
                    // _unitOfWorkAsync.SaveChangesAsync();
                    var test = _unitOfWorkAsync.Orphanages.GetById(orphanage.ID).Result;
                    //Orphanage.CopyProperties(test, orphanage); -> копируем состояние
                    //_unitOfWorkAsync.Orphanages.Update(test);
                    test.AdressID = 1;//.City = orphanage.Adress.City;
                    _unitOfWorkAsync.Orphanages.Update(test);
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
            _unitOfWorkAsync.Orphanages.Delete(orphanage.ID);
            _unitOfWorkAsync.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool OrphanageExists(int id) =>
            _unitOfWorkAsync.Orphanages.GetById(id) != null;
    }
}
