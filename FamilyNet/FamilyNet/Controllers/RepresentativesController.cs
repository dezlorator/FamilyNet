using System;
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
using FamilyNet.Models.ViewModels;
using FamilyNet.Infrastructure;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class RepresentativesController : BaseController
    {
        #region Ctor

        public RepresentativesController(IUnitOfWorkAsync unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        #region Methods

        // GET: Representatives
        /// <summary>
        /// Method provides list of Representatives
        /// </summary>
        /// <param name="id">Representative's identifier</param>
        /// <param name="searchModel">parameters for filter</param>
        /// <returns>view with list of Representatives</returns>
        [AllowAnonymous]
        public async Task<IActionResult> Index(int id,
            PersonSearchModel searchModel)
        {
            IEnumerable<Representative> representatives = _unitOfWorkAsync.Representatives.GetAll();

            representatives = RepresentativeFilter.GetFiltered(representatives, searchModel);

            if (id == 0)
                return View(representatives);

            if (id > 0)
                representatives = representatives.Where(x => x.Orphanage.ID.Equals(id));

            return View(representatives);
        }

        // GET: Representatives/Details/5
        /// <summary>
        /// Method provides information about Representative by id
        /// </summary>
        /// <param name="id">Representative's identifier</param>
        /// <returns>view with details about Representatives or returns NotFound page</returns>
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

        // GET: Representatives/Create
        /// <summary>
        /// Method provides view for add new Representative into database
        /// </summary>
        /// <returns>view with empty form</returns>
        [Authorize(Roles = "Admin, Representative")]
        public async Task<IActionResult> Create()
        {
            await Check();

            List<Orphanage> orphanages = await _unitOfWorkAsync.Orphanages.GetAll()
                .OrderBy(o => o.Name).ToListAsync();

            ViewBag.Orphanages = new SelectList(orphanages, "ID", "Name");

            return View();
        }



        // POST: Representatives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Method validates and adds new Representative into database
        /// </summary>
        /// <param name="representative">Representative's adding model</param>
        /// <param name="id">Representative's identifier</param>
        /// <param name="file">Representative's photo</param>
        /// <returns>Redirect to Index if model is valid or return to Create page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Representative")]
        public async Task<IActionResult> Create([Bind("FullName,Birthday,Rating,Avatar,Orphanage")] 
        Representative representative, int id, IFormFile file)
        {
            await ImageHelper.SetAvatar(representative, file, "wwwroot\\representatives");

            if (ModelState.IsValid)
            {
                var orphanage = await _unitOfWorkAsync.Orphanages.GetById(id);
                representative.Orphanage = orphanage;

                await _unitOfWorkAsync.Representatives.Create(representative);
                await _unitOfWorkAsync.Representatives.SaveChangesAsync();

                var user = await GetCurrentUserAsync();
                user.PersonID = representative.ID;
                user.PersonType = Models.Identity.PersonType.Representative;
                await _unitOfWorkAsync.UserManager.UpdateAsync(user);

                return RedirectToAction(nameof(Index));
            }
            return View(representative);
        }

        // GET: Representatives/Edit/5
        /// <summary>
        ///  Method provides view for edit Representative
        /// </summary>
        /// <param name="id">Representative's identifier</param>
        /// <returns>view with editing form for Representative</returns>
        [Authorize(Roles = "Admin, Representative")]
        public async Task<IActionResult> Edit(int? id)
        {

            List<Orphanage> orphanages = _unitOfWorkAsync.Orphanages.GetAll()
                .OrderBy(o => o.Name).ToList();
            ViewBag.Orphanages = new SelectList(orphanages, "ID", "Name");


            if (id == null)
                return NotFound();

            var check = CheckById((int)id).Result;
            var checkResult = check != null;
            if (checkResult)
            {
                return check;
            }

           

            var representative = await _unitOfWorkAsync.Representatives.GetById((int)id);

            if (representative == null)
                return NotFound();

            return View(representative);
        }

        // POST: Representatives/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Method validates and updates Representative in database
        /// </summary>
        /// <param name="id">Representative's identifier</param>
        /// <param name="representative">Representative's details by id</param>
        /// <param name="orphanageId">identifier of Orphanage</param>
        /// <param name="file">Representative's photo</param>
        /// <returns>Redirect to index if model is valid or return to Edit page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Representative")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FullName,Birthday,Rating,Avatar,Orphanage")]
         Representative representative, int orphanageId, IFormFile file)
        {
            if (id != representative.ID)
                return NotFound();

            var check = CheckById((int)id).Result;
            var checkResult = check != null;
            if (checkResult)
            {
                return check;
            }

            await ImageHelper.SetAvatar(representative, file, "wwwroot\\representatives");

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
                    if (!_unitOfWorkAsync.Representatives.Any(representative.ID))
                        return NotFound();
                    else
                        throw; //TODO: Loging
                }

                return RedirectToAction(nameof(Index));
            }

            return View(representative);
        }

        // GET: Representatives/Delete/5
        /// <summary>
        /// Method provides page for deletes Representative by id from database
        /// </summary>
        /// <param name="id">Representative's identifier</param>
        /// <returns>view delete if model is valid or redirect to NotFound page</returns>
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
        /// <summary>
        /// Method deletes Representative by id from database
        /// </summary>
        /// <param name="id">Representative's identifier</param>
        /// <returns>Redirect to Index page</returns>
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

        #endregion
    }
}