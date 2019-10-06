using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Models.ViewModels;
using FamilyNet.Infrastructure;

using Microsoft.Extensions.Localization;

namespace FamilyNet.Controllers
{
    /// <summary>
    /// The controller accepts and processes requests about Orphans
    /// </summary>
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
        /// <summary>
        /// Method provides list of Orphans
        /// </summary>
        /// <param name="id">Orphan's identifier</param>
        /// <param name="searchModel">parameters for filter</param>
        /// <returns>view with list of Orphans</returns>
        [AllowAnonymous]
        public async Task<IActionResult> Index(int id, PersonSearchModel searchModel)
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

        // GET: Orphans/Details/5
        //[HttpGet("[controller]/[action]/{id}")]
        /// <summary>
        /// Method provides information about Orphan by id
        /// </summary>
        /// <param name="id">Orphan's identifier</param>
        /// <returns>view with details about Orphan or returns NotFound page</returns>
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
        /// <summary>
        /// Method provides view for add new Orphan into database
        /// </summary>
        /// <returns>view with inputs</returns>
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
        /// <summary>
        /// Method validates and adds new Orphan into database
        /// </summary>
        /// <param name="orphan">adding entity</param>
        /// <param name="id">Orphan's identifier</param>
        /// <param name="file">Orphan's photo</param>
        /// <returns>Redirect to Index if model is valid or return to Create page</returns>
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
        /// <summary>
        ///  Method provides view for edit Orphan
        /// </summary>
        /// <param name="id">Orphan's identifier</param>
        /// <returns>view with editing entity</returns>
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
        /// <summary>
        /// Method validates and updates Orphan in database
        /// </summary>
        /// <param name="id">Orphan's identifier</param>
        /// <param name="orphan">Orphon's details by id</param>
        /// <param name="idOrphanage">identifier of Orphanage</param>
        /// <param name="file">Orphan's photo</param>
        /// <returns>Redirect to index if model is valid or return to Edit page</returns>
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
        /// <summary>
        /// Method provides page for deletes Orphan by id from database
        /// </summary>
        /// <param name="id">Orphan's identifier</param>
        /// <returns>view delete if model is valid or redirect to NotFound page</returns>
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
        /// <summary>
        /// Method deletes Orphan by id from database
        /// </summary>
        /// <param name="id">Orphan's identifier</param>
        /// <returns>Redirect to Index page</returns>
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
        /// <summary>
        /// Method provides table of Orphans
        /// </summary>
        /// <param name="id">Orphan's identifier</param>
        /// <param name="searchModel">filter parameters</param>
        /// <returns>view table</returns>
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

        /// <summary>
        /// Method sets to ViewData localized strings
        /// </summary>
        private void GetViewData()
        {
            ViewData["OrphansList"] = _localizer["OrphansList"];
        }

        /// <summary>
        /// Method checks existing Orphan by id
        /// </summary>
        /// <param name="id">identifier of Orphan</param>
        /// <returns>true if Orphan with id is exists</returns>
        private bool OrphanExists(int id)
        {
            return _unitOfWorkAsync.Orphans.GetById(id) != null;
        }


    }
}
