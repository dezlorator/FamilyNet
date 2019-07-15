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
    public class CharityMakersController : BaseController
    {
        public CharityMakersController(IUnitOfWorkAsync unitOfWork) : base (unitOfWork)
        { }

        // GET: CharityMakers
        public async Task<IActionResult> Index()
        {
            var list = await _unitOfWorkAsync.CharityMakers.GetAll().ToListAsync();
            return View(list);
        }

        // GET: CharityMakers/Details/5
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: CharityMakers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Birthday,Rating")] CharityMaker charityMaker)
        {
            if (ModelState.IsValid)
            {
                charityMaker.Address = new Address() { City = "contr", House = "contr", Country = "contr", Region = "contr", Street = "contr" };
                await _unitOfWorkAsync.CharityMakers.Create(charityMaker);
                await _unitOfWorkAsync.CharityMakers.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(charityMaker);
        }

        // GET: CharityMakers/Edit/5
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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Birthday,Rating")] CharityMaker charityMaker)
        {
            if (id != charityMaker.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(charityMaker);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CharityMakerExists(charityMaker.ID))
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
            return View(charityMaker);
        }

        // GET: CharityMakers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CharityMaker charityMaker = null;// await _context.CharityMakers
                //.FirstOrDefaultAsync(m => m.ID == id);
            if (charityMaker == null)
            {
                return NotFound();
            }

            return View(charityMaker);
        }

        

        private bool CharityMakerExists(int id)
        {
            return _unitOfWorkAsync.CharityMakers.GetById(id) != null;
        }
    }
}
