using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class RepresentativesController : BaseController
    {
        #region Ctor

        public RepresentativesController(IUnitOfWorkAsync unitOfWork) 
            : base(unitOfWork)
        {
        }
        #endregion

        #region Methods

        // GET: Representatives
        [AllowAnonymous]
        public async Task<IActionResult> Index(int id)
        {
            var orphans = _unitOfWorkAsync.Representatives.GetAll();

            if (id == 0)
                return View(await orphans.ToListAsync());

            if (id > 0)
                orphans = orphans.Where(x => x.Orphanage.ID.Equals(id));

            return View(await orphans.ToListAsync());
        }

        // GET: Representatives/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var representative = await _unitOfWorkAsync.Representatives.GetById((int)id);

            if (representative == null)
                return NotFound();

            return View(representative);
        }
        [Authorize(Roles = "Admin")]
        // GET: Representatives/Create
        public async Task<IActionResult> Create()
        {
            List<Orphanage> orphanages = await _unitOfWorkAsync.Orphanages.GetAll()
                .OrderBy(o => o.Name).ToListAsync();
            ViewBag.Orphanages = new SelectList(orphanages, "ID", "Name");

            return View();
        }

        // POST: Representatives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [Bind("FullName,Birthday,Rating,Avatar,Orphanage")] Representative representative,
            int id, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetRandomFileName();
                fileName = Path.ChangeExtension(fileName, ".jpg");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot\\representatives", fileName);
                using (var fileSteam = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileSteam);
                }
                representative.Avatar = fileName;
            }

            if (ModelState.IsValid)
            {
                var orphanage = await _unitOfWorkAsync.Orphanages.GetById(id);
                representative.Orphanage = orphanage;

                await _unitOfWorkAsync.Representatives.Create(representative);
                await _unitOfWorkAsync.Representatives.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(representative);
        }

        // GET: Representatives/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            List<Orphanage> orphanages = _unitOfWorkAsync.Orphanages.GetAll()
                .OrderBy(o => o.Name).ToList();
            ViewBag.Orphanages = new SelectList(orphanages, "ID", "Name");

            if (id == null)
                return NotFound();

            var representative = await _unitOfWorkAsync.Representatives.GetById((int)id);

            if (representative == null)
                return NotFound();

            return View(representative);
        }

        // POST: Representatives/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, 
            [Bind("ID,FullName,Birthday,Rating,Avatar,Orphanage")] Representative representative,
            int orphanageId, IFormFile file)
        {
            if (id != representative.ID)
                return NotFound();

            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetRandomFileName();
                fileName = Path.ChangeExtension(fileName, ".jpg");
                var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot\\representatives", fileName);
                using (var fileSteam = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileSteam);
                }
                representative.Avatar = fileName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var orphanage = await _unitOfWorkAsync.Orphanages.GetById(orphanageId);
                    representative.Orphanage = orphanage;

                    var representativeToEdit = await _unitOfWorkAsync.Representatives.GetById(representative.ID);
                    representativeToEdit.CopyState(representative);
                    _unitOfWorkAsync.Representatives.Update(representativeToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RepresentativeExists(representative.ID))
                        return NotFound();
                    else
                        throw; //TODO: Loging
                }

                return RedirectToAction(nameof(Index));
            }

            return View(representative);
        }

        // GET: Representatives/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var representative = await _unitOfWorkAsync.Representatives.GetById((int)id);
            if (representative == null)
                return NotFound();

            return View(representative);
        }

        // POST: Representatives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var representative = await _unitOfWorkAsync.Representatives.GetById((int)id);
            await _unitOfWorkAsync.Representatives.Delete(representative.ID);
            _unitOfWorkAsync.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool RepresentativeExists(int id)
        {
            return _unitOfWorkAsync.Representatives.GetById(id) != null;
        }

        #endregion
    }
}
