using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Models.ViewModels;
using Microsoft.Extensions.Localization;
using System.Globalization;
using FamilyNet.Infrastructure;
using System;
using System.Collections.Generic;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class OrphanagesController : BaseController
    {
        #region Private fields

        //первая инициализация в методе index
        /// <summary>
        /// Search model
        /// </summary>
        private OrphanageSearchModel _searchModel;

        //это кажется не надо. Не используется в этом контроллере
        private readonly IHostingEnvironment _hostingEnvironment;

        /// <summary>
        /// Stores localization related data
        /// </summary>
        private readonly IStringLocalizer<OrphanagesController> _localizer;

        public OrphanagesController(IUnitOfWorkAsync unitOfWork, IHostingEnvironment environment,
            IStringLocalizer<OrphanagesController> localizer, IStringLocalizer<SharedResource> sharedLocalizer)
            : base(unitOfWork, sharedLocalizer)
        {
            _hostingEnvironment = environment;
            _localizer = localizer;
        }

        //это нигде не используется
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

        #endregion

        #region ActionMethods

        // GET: Orphanages
        /// <summary>
        /// Gets a list of all orphanages by the id
        /// </summary>
        /// <param name="id">Unique orphanages id</param>
        /// <param name="searchModel">Search model</param>
        /// <param name="sortOrder">Sort option</param>
        /// <returns>view with IQueryable<Orphanage></returns>
        [AllowAnonymous]
        public async Task<IActionResult> Index(int id, OrphanageSearchModel searchModel,
            SortStateOrphanages sortOrder = SortStateOrphanages.NameAsc)
        {
            IQueryable<Orphanage> orphanages = _unitOfWorkAsync.Orphanages.GetAll();

            //выполняем фильтрацию
            orphanages = GetFiltered(orphanages, searchModel);
            //сортируем
            orphanages = GetSorted(orphanages, sortOrder);

            //выводим много много много абсолютно все приюты
            if (id == 0)
                // можно убрать ToListAsync()
                return View(await orphanages.ToListAsync());

            //выводим только один приют
            if (id > 0)
                orphanages = orphanages.Where(x => x.ID.Equals(id));

            // можно убрать ToListAsync() ????
            return View(await orphanages.ToListAsync());
        }


        // GET: Orphanages/Details/5
        /// <summary>
        /// Displays the orphanage indicated by id
        /// </summary>
        /// <param name="id">Unique orphanages id</param>
        /// <returns>view with Orphanage</returns>
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);

            if (orphanage == null)
                return NotFound();
            GetViewData();

            return View(orphanage);
        }

        // GET: Orphanages/Create
        /// <summary>
        /// Display a form for entering orphanage data
        /// </summary>
        /// <returns>view</returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // POST: Orphanages/Create
        /// <summary>
        /// Adds a new Orphanage to the database
        /// </summary>
        /// <param name="orphanage">Added Orphanage</param>
        /// <param name="file">Object to upload a file to the server</param>
        /// <returns>view with orphanage</returns>
        [HttpPost]
        //для валидации
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Adress,Rating,Avatar")] Orphanage orphanage,
            IFormFile file) //TODO: AlPa -> Research Bind To Annotations
        {
            //file нужен для загрузки файлов на сервер
            //установка аватара
            await ImageHelper.SetAvatar(orphanage, file, "wwwroot\\avatars");

            //вычисляем координаты по введенным данным
            //part to add location when obj creating
            bool IsLocationNotNull = GetCoordProp(orphanage.Adress, out var Location);
            if (IsLocationNotNull)
            {
                orphanage.Location = new Location()
                {
                    MapCoordX = Location.Item1,
                    MapCoordY = Location.Item2
                };
            }
            else
                orphanage.LocationID = null;

            //если все ок, то добавляем в бд и возвращемся в главный метод
            if (ModelState.IsValid)
            {
                await _unitOfWorkAsync.Orphanages.Create(orphanage);
                await _unitOfWorkAsync.Orphanages.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            GetViewData();

            //если не ок, то вводим данные заново
            return View(orphanage);
        }

        // GET: Orphanages/Edit/5
        /// <summary>
        /// Displays a form for editing a specific orphanage specified by id
        /// </summary>
        /// <param name="id">Unique orphanages id</param>
        /// <returns> view with orphanage </returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);

            if (orphanage == null)
                return NotFound();
            GetViewData();

            return View(orphanage);
        }

        // POST: Orphanages/Edit/5
        /// <summary>
        /// Edits a specific orphanage specified by id
        /// </summary>
        /// <param name="orphanage">edited object</param>
        /// <param name="id">Unique orphanages id</param>
        /// <param name="file">Object to upload a file to the server</param>
        /// <returns>view with orphanages</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Representative")]
        public async Task<IActionResult> Edit([Bind("ID,Name,Adress,Rating,Avatar")]
            Orphanage orphanage, int id, IFormFile file) //TODO: Check change id position
        {
            if (id != orphanage.ID)
                return NotFound();

            await ImageHelper.SetAvatar(orphanage, file, "wwwroot\\avatars");

            if (ModelState.IsValid)
            {
                try
                {
                    //in ef to change the object you need to track it out of context
                    var orphanageToEdit = await _unitOfWorkAsync.Orphanages.GetById(orphanage.ID);

                    //copying the state with NOT CHANGING REFERENCES
                    orphanageToEdit.CopyState(orphanage);

                    //edit location
                    bool IsLocationNotNull = GetCoordProp(orphanage.Adress, out var Location);
                    if (IsLocationNotNull)
                    {
                        orphanageToEdit.Location = new Location()
                        {
                            MapCoordX = Location.Item1,
                            MapCoordY = Location.Item2
                        };
                    }
                    else
                        orphanageToEdit.LocationID = null;

                    _unitOfWorkAsync.Orphanages.Update(orphanageToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_unitOfWorkAsync.Orphanages.Any(orphanage.ID))
                        return NotFound();
                    else
                        throw; //TODO: Loging
                }

                return RedirectToAction(nameof(Index));
            }
            GetViewData();

            return View(orphanage);
        }

        // GET: Orphanages/Delete/5
        /// <summary>
        /// Displays a form for removing a specific orphanage at the specified id
        /// </summary>
        /// <param name="id">Unique orphanages id</param>
        /// <returns>view with orphanages</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var orphanage = await _unitOfWorkAsync.Orphanages.GetById((int)id);

            if (orphanage == null)
                return NotFound();
            GetViewData();

            return View(orphanage);
        }

        // POST: Orphanages/Delete/5
        /// <summary>
        /// Deletes the selected orphanage from id from the database
        /// </summary>
        /// <param name="id">Unique orphanages id</param>
        /// <returns>redirect to index view</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orphanage = await _unitOfWorkAsync.Orphanages.GetById(id);
            await _unitOfWorkAsync.Orphanages.Delete(orphanage.ID);
            _unitOfWorkAsync.SaveChangesAsync();
            GetViewData();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays a form for finding orphanage by type of assistance
        /// </summary>
        /// <returns>view</returns>
        [AllowAnonymous]
        public IActionResult SearchByTypeHelp()
        {
            GetViewData();
            return View();
        }

        /// <summary>
        /// Searches for orphanage by type of help entered
        /// </summary>
        /// <returns>view with orphanage list</returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult SearchResult(string typeHelp)
        {
            ViewData["TypeHelp"] = typeHelp;
            IEnumerable<Orphanage> list = new List<Orphanage>();
            if (typeHelp != null)
                list = _unitOfWorkAsync.Orphanages.Get(
                    orp => orp.Donations.Where(
                        donat => donat.DonationItem.TypeBaseItem.Where(
                            donatitem => donatitem.Type.Name.ToLower().Contains(typeHelp.ToLower())).
                            Count() > 0 && donat.IsRequest).
                        Count() > 0);
            GetViewData();

            return View("SearchResult", list);
        }

        /// <summary>
        /// Shows a map with orphanages
        /// </summary>
        /// <returns>view with all orphanages</returns>
        [AllowAnonymous]
        public IActionResult SearchOrphanageOnMap()
        {
            var orphanages = _unitOfWorkAsync.Orphanages.GetForSearchOrphanageOnMap();
            GetViewData();

            return View(orphanages);
        }


        #endregion

        #region Private Helpers

        /// <summary>
        /// Does Address contain the specified string
        /// </summary>
        /// <param name="addr">Full address format</param>
        /// <returns>Returns true if contains</returns>
        private bool Contains(Address addr)
        {
            foreach (var word in _searchModel.AddressString.Split())
            {
                string wordUpper = word.ToUpper();

                if (addr.Street.ToUpper().Contains(wordUpper)
                        || addr.City.ToUpper().Contains(wordUpper)
                        || addr.Region.ToUpper().Contains(wordUpper)
                        || addr.Country.ToUpper().Contains(wordUpper))
                    return true;
            }

            return false;
        }

        /// <summary>
        ///Sorts orphanages by specified criteria
        /// </summary>
        /// <param name="orphanages">Orphanages list</param>
        /// <param name="sortOrder">Sorting criteria</param>
        /// <returns>Returns a sorted orphanages list</returns>
        private IQueryable<Orphanage> GetSorted(IQueryable<Orphanage> orphanages, SortStateOrphanages sortOrder)
        {
            //данные для ui
            ViewData["NameSort"] = sortOrder == SortStateOrphanages.NameAsc
                ? SortStateOrphanages.NameDesc : SortStateOrphanages.NameAsc;
            ViewData["AddressSort"] = sortOrder == SortStateOrphanages.AddressAsc
                ? SortStateOrphanages.AddressDesc : SortStateOrphanages.AddressAsc;
            ViewData["RatingSort"] = sortOrder == SortStateOrphanages.RatingAsc
                ? SortStateOrphanages.RatingDesc : SortStateOrphanages.RatingAsc;

            //в зависимости от состояния выпоняем сортировку
            switch (sortOrder)
            {
                case SortStateOrphanages.NameDesc:
                    orphanages = orphanages.OrderByDescending(s => s.Name);
                    break;
                case SortStateOrphanages.AddressAsc:
                    orphanages = orphanages
                        .OrderBy(s => s.Adress.Country)
                        .ThenBy(s => s.Adress.Region)
                        .ThenBy(s => s.Adress.City)
                        .ThenBy(s => s.Adress.Street);
                    break;
                case SortStateOrphanages.AddressDesc:
                    orphanages = orphanages
                        .OrderByDescending(s => s.Adress.Country)
                        .ThenByDescending(s => s.Adress.Region)
                        .ThenByDescending(s => s.Adress.City)
                        .ThenByDescending(s => s.Adress.Street);
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

            return orphanages;
        }

        /// <summary>
        /// Selects orphanages by a given filter
        /// </summary>
        /// <param name="orphanages">Orphanages list</param>
        /// <param name="searchModel">Filter for the orphanages selection</param>
        /// <returns>Returns orphanages list</returns>
        private IQueryable<Orphanage> GetFiltered(IQueryable<Orphanage> orphanages,
            OrphanageSearchModel searchModel)
        {
            if (searchModel != null)
            {
                _searchModel = searchModel;

                //ищем дет дома по указ имени
                if (!string.IsNullOrEmpty(searchModel.NameString))
                    orphanages = orphanages.Where(x => x.Name.Contains(searchModel.NameString));

                //ищем дет дома по указ адресу
                if (!string.IsNullOrEmpty(searchModel.AddressString))
                    orphanages = orphanages.Where(x => Contains(x.Adress));

                //ищем по рейтингу
                if (searchModel.RatingNumber > 0)
                    orphanages = orphanages.Where(x => x.Rating >= searchModel.RatingNumber);
            }
            GetViewData();

            return orphanages;
        }

        /// <summary>
        /// Calculates the coordinates at the specified address
        /// </summary>
        /// <param name="address">Address to calculate</param>
        /// <param name="result">Tuple with coordinates</param>
        /// <returns>Returns true if coordinates were calculated</returns>
        private bool GetCoordProp(Address address, out Tuple<float?, float?> result)
        {
            result = null;
            bool forOut = false;

            //геокодирование !?
            var nominatim = new Nominatim.API.Geocoders.ForwardGeocoder();
            //формируем геоданные
            var d = nominatim.Geocode(new Nominatim.API.Models.ForwardGeocodeRequest()
            {
                Country = address.Country,
                State = address.Region,
                City = address.City,
                StreetAddress = String.Concat(address.Street, " ", address.House)
            });

            //TODO:some validation for search
            if (d.Result.Count() != 0)
            {
                float? X = (float)d.Result[0].Latitude;
                float? Y = (float)d.Result[0].Longitude;

                result = new Tuple<float?, float?>(X, Y);
                forOut = true;
            }

            return forOut;
        }

        /// <summary>
        /// Initializes ViewData and transfers localization data to ui
        /// </summary>
        private void GetViewData()
        {
            ViewData["Name"] = _localizer["Name"];
            ViewData["Rating"] = _localizer["Rating"];
            ViewData["Photo"] = _localizer["Photo"];
            ViewData["Actions"] = _localizer["Actions"];
            ViewData["SaveChanges"] = _localizer["SaveChanges"];

            ViewData["ReturnToList"] = _localizer["ReturnToList"];
            ViewData["Details"] = _localizer["Details"];
            ViewData["Profile"] = _localizer["Profile"];
            ViewData["Address"] = _localizer["Address"];
            ViewData["From"] = _localizer["From"];
            @ViewData["ListOrphanages"] = _localizer["ListOrphanages"];
        }

        #endregion
    }
}

