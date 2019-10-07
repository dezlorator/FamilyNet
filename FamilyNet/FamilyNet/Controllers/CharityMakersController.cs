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
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Models.ViewModels;
using FamilyNet.Infrastructure;

namespace FamilyNet.Controllers
{
    /// <summary>
    /// This controller gets and procecced requests about charity makers
    /// </summary>
    [Authorize]
    public class CharityMakersController : BaseController
    {
        public CharityMakersController(IUnitOfWorkAsync unitOfWork) : base (unitOfWork)
        {

        }

        // GET: CharityMakers
        /// <summary>
        /// Method provides list of charity makers
        /// </summary>
        /// <param name="searchModel">parameter for filtre charity makers</param>
        /// <returns>Index view with list of charity makers</returns>
        [AllowAnonymous]
        //TODO: Make async
        public async Task<IActionResult> Index(PersonSearchModel searchModel)
        {
            IEnumerable<CharityMaker> charityMakers =  _unitOfWorkAsync.CharityMakers.GetAll();
            //TODO: Static method???
            charityMakers = CharityMakerFilter.GetFiltered(charityMakers, searchModel);

            return View(charityMakers);
        }

        // GET: CharityMakers/Details/5
        /// <summary>
        /// Method provides detail information about charity makers by id
        /// </summary>
        /// <param name="id">parameter for identifying charity maker</param>
        /// <returns>Returns Details view with current charity maker 
        /// or return not found if charity maker does not exist</returns>
        [AllowAnonymous]
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
        /// <summary>
        /// Method provides view to add user, if this user is admin or charity maker
        /// </summary>
        /// <returns>Create view</returns>
        [Authorize(Roles = "Admin, CharityMaker")]
        public IActionResult Create()
        {
            Check();
            return View();
        }

        // POST: CharityMakers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Method provides creating new charity maker
        /// </summary>
        /// <param name="charityMaker">parameter with info about new charity maker</param>
        /// <returns>Redirect to Index if is added or Create view if didn`t</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, CharityMaker")]
        public async Task<IActionResult> Create([Bind("ID,FullName,Address,Birthday,Contacts,Rating")] CharityMaker charityMaker)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWorkAsync.CharityMakers.Create(charityMaker);
                await _unitOfWorkAsync.CharityMakers.SaveChangesAsync();

                var user = await GetCurrentUserAsync();
                user.PersonID = charityMaker.ID;
                user.PersonType = Models.Identity.PersonType.CharityMaker;
                await _unitOfWorkAsync.UserManager.UpdateAsync(user);

                return RedirectToAction(nameof(Index));
            }
            return View(charityMaker);
        }

        // GET: CharityMakers/Edit/5
        /// <summary>
        /// Method provides getting Edit view with charity maker id
        /// </summary>
        /// <param name="id">parameter to identify charity maker, which you want to edit</param>
        /// <returns>Edit view if id exists or not found if doesn`t</returns>
        [Authorize(Roles = "Admin, CharityMaker")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //TODO: Make code shorter
            var check = CheckById((int)id).Result;
            var checkResult = check != null;
            if(checkResult)
            {
                return check;
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
        /// <summary>
        /// Method provides editing charity maker
        /// </summary>
        /// <param name="id">charity maker`s id, who you want to change</param>
        /// <param name="charityMaker">new info about charity maker</param>
        /// <returns>Redirect to index when added successfully, not found if id not found
        /// or CharityMaker view if info about charity maker is invalid</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, CharityMaker")]
        public async Task<IActionResult> Edit(int id,
            [Bind("ID,FullName,Address,Birthday,Contacts,Rating")] CharityMaker charityMaker)
        {
            var check = CheckById((int)id).Result;
            //TODO: Make code shorter
            var checkResult = check != null;
            if (checkResult)
            {
                return check;
            }

            if (id != charityMaker.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    CharityMaker charityMakerToEdit = await _unitOfWorkAsync.CharityMakers.GetById(id);
                    charityMakerToEdit.CopyState(charityMaker);
                    _unitOfWorkAsync.CharityMakers.Update(charityMakerToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_unitOfWorkAsync.CharityMakers.Any(charityMaker.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // TODO : logging
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(charityMaker);
        }

        // GET: CharityMakers/Delete/5
        /// <summary>
        /// Method provides getting Delete view with charity maker 
        /// </summary>
        /// <param name="id">charity maker identifier</param>
        /// <returns>Delete view if id exists or not found if don`t</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CharityMaker charityMaker = await _unitOfWorkAsync.CharityMakers
                .GetById((int)id);

            if (charityMaker == null)
            {
                return NotFound();
            }

            return View(charityMaker);
        }

        /// <summary>
        /// Method provides deleting charity maker
        /// </summary>
        /// <param name="id">charity maker identifier</param>
        /// <returns>Redirect to Index view</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!_unitOfWorkAsync.CharityMakers.Any(id))
            {
                await _unitOfWorkAsync.CharityMakers.Delete(id);
                _unitOfWorkAsync.SaveChangesAsync();
            }          

            return RedirectToAction(nameof(Index));
        }

        private bool HasCharityMaker(int id)
        {
            return _unitOfWorkAsync.CharityMakers.GetById(id) != null;
        }
        /// <summary>
        /// Method provides getting Table view with all charity makers
        /// </summary>
        /// <returns>Table view with list of charity makers</returns>
        // GET: CharityMakers/Table
        [Authorize(Roles = "Admin")]
        public IActionResult Table()
        {
            var list = _unitOfWorkAsync.CharityMakers.GetAll().ToList();
            return View(list);
        }

    }
}
