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
using FamilyNet.Models.ViewModels;
using FamilyNet.Infrastructure;

using Microsoft.Extensions.Localization;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class OrphansController : BaseController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<OrphansController> _localizer;

        public OrphansController(IUnitOfWorkAsync unitOfWork, IHostingEnvironment environment, IStringLocalizer<OrphansController> localizer) : base(unitOfWork)
        {
            _hostingEnvironment = environment;
            _localizer = localizer;
        }

        // GET: Orphans
        [AllowAnonymous]
        public async Task<IActionResult> Index(int id, PersonSearchModel searchModel)
        {
            IEnumerable<Orphan> orphans = _unitOfWorkAsync.Orphans.GetAll();

            orphans = OrphanFilter.GetFiltered(orphans, searchModel);

            if (id == 0)
                return View(orphans);

            if (id >0)
                orphans = orphans.Where(x => x.Orphanage.ID.Equals(id)).ToList();
            GetViewData();

            return View(orphans);
        }

        // GET: Orphans/Details/5
        //[HttpGet("[controller]/[action]/{id}")]
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
            GetViewData();

            return View(orphan);
        }

        // GET: Orphans/Create
        [Authorize(Roles ="Admin, Orphan")]
        public IActionResult Create()
        {
            Check();

            List<Orphanage> orphanagesList = new List<Orphanage>();
            orphanagesList = _unitOfWorkAsync.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;
            GetViewData();

            return View();
        }

        // POST: Orphans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Create([Bind("FullName,Address,Birthday,Orphanage,Avatar")] Orphan orphan, int id, IFormFile file)
        {
            await ImageHelper.SetAvatar(orphan, file, "wwwroot\\children");

            if (ModelState.IsValid)
            {
                var orphanage = await _unitOfWorkAsync.Orphanages.GetById(id);
                orphan.Orphanage = orphanage;

                await _unitOfWorkAsync.Orphans.Create(orphan);
                await _unitOfWorkAsync.Orphans.SaveChangesAsync();

                var user = await GetCurrentUserAsync();
                user.PersonID = orphan.ID;
                user.PersonType = Models.Identity.PersonType.Orphan;
                await _unitOfWorkAsync.UserManager.UpdateAsync(user);
                

               
                return RedirectToAction(nameof(Index));
            }
            GetViewData();

            return View(orphan);
        }

        // GET: Orphans/Edit/5
        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Edit(int? id)
        {

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

            List<Orphanage> orphanagesList = _unitOfWorkAsync.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;

            var orphan = await _unitOfWorkAsync.Orphans.GetById((int)id);

            if (orphan == null)
            {
                return NotFound();
            }
            GetViewData();

            return View(orphan);
        }

        // POST: Orphans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FullName,Birthday,Orphanage,Avatar")] Orphan orphan, int idOrphanage, IFormFile file)
        {
            if (id != orphan.ID)
            {
                return NotFound();
            }

            var check = CheckById((int)id).Result;
            var checkResult = check != null;
            if (checkResult)
            {
                return check;
            }

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
            GetViewData();

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
            GetViewData();

            return View(orphan);
        }

        // POST: Orphans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orphan = await _unitOfWorkAsync.Orphans.GetById((int)id);
            if (orphan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            await _unitOfWorkAsync.Orphans.Delete((int)id);
            _unitOfWorkAsync.SaveChangesAsync();
            GetViewData();

            return RedirectToAction(nameof(Index));
        }

        // GET: Orphans/OrphansTable
        [AllowAnonymous]
        public IActionResult OrphansTable(int id, PersonSearchModel searchModel)
        {
            IEnumerable<Orphan> orphans = _unitOfWorkAsync.Orphans.GetAll();

            orphans = OrphanFilter.GetFiltered(orphans, searchModel);

            if (id == 0)
                return View(orphans);

            if (id > 0)
                orphans = orphans.Where(x => x.Orphanage.ID.Equals(id)).ToList();

            GetViewData();

            return View(orphans);
        }

        private void GetViewData()
        {
            ViewData["OrphansList"] = _localizer["OrphansList"];
        }

        private bool OrphanExists(int id)
        {
            return _unitOfWorkAsync.Orphans.GetById(id) != null;
        }


    }
}
