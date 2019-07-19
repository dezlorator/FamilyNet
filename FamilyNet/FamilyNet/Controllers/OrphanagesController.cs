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
        public OrphanagesController(IUnitOfWorkAsync unitOfWork) : base(unitOfWork)
        { }

        // GET: Orphanages
        public async Task<IActionResult> Index(string name, SortStateOrphanages sortOrder = SortStateOrphanages.NameAsc)
        {
            IQueryable<Orphanage> orphanages = _unitOfWorkAsync.Orphanages.GetAll();

            if (!String.IsNullOrEmpty(name))
            {
                orphanages = orphanages.Where(p => p.Name.Contains(name));
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
                    orphanages = orphanages.OrderBy(s => s.Adress);
                    break;
                case SortStateOrphanages.AddressDesc:
                    orphanages = orphanages.OrderByDescending(s => s.Adress);
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
            //var list =  _unitOfWorkAsync.Orphanages.GetAll().ToList();
            //return View(list);
            FamilyNet.Models.Filters.OrphanagesViewModel viewModel = new FamilyNet.Models.Filters.OrphanagesViewModel
            {
                Orphanages = orphanages.ToList(),
                //Companies = new SelectList(companies, "Id", "Name"),
                Name = name
            };
            //return View(viewModel);
            return View(await orphanages.ToListAsync());
        }

        //public ActionResult Index(/*int? company,*/ string name)
        //{
        //    IQueryable<Orphanage> orphanages = _unitOfWorkAsync.Orphanages.GetAll();
        //    //if (company != null && company != 0)
        //    //{
        //    //    users = users.Where(p => p.CompanyId == company);
        //    //}
        //    if (!String.IsNullOrEmpty(name))
        //    {
        //        orphanages = orphanages.Where(p => p.Name.Contains(name));
        //    }

        //    //List<Company> companies = db.Companies.ToList();
        //    //companies.Insert(0, new Company { Name = "Все", Id = 0 });

        //    FamilyNet.Models.Filters.OrphanagesViewModel viewModel = new FamilyNet.Models.Filters.OrphanagesViewModel
        //    {
        //        Orphanages = orphanages.ToList(),
        //        //Companies = new SelectList(companies, "Id", "Name"),
        //        Name = name
        //    };
        //    return View(viewModel);
        //}

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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Adress,Rating,Avatar")] Orphanage orphanage)
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
            ////in ef to change the object you need to track it out of context
            //var orphanageToEdit = await _unitOfWorkAsync.Orphanages.GetById(orphanage.ID);
            ////copying the state with NOT CHANGING REFERENCES
            //Orphanage.CopyState(orphanageToEdit, orphanage);
            //_unitOfWorkAsync.Orphanages.Update(orphanageToEdit);
            //_unitOfWorkAsync.SaveChangesAsync();
            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);
            await _unitOfWorkAsync.Orphanages.Delete(orphanage.ID);
            _unitOfWorkAsync.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool OrphanageExists(int id) =>
            _unitOfWorkAsync.Orphanages.GetById(id) != null;
    }
}
