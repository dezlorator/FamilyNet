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
using FamilyNet.Downloader;
using DataTransferObjects;
using FamilyNet.StreamCreater;
using System.Net.Http;
using Newtonsoft.Json;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class OrphanagesController : BaseController
    {
        #region Private fields

        private OrphanageSearchModel _searchModel;

        private readonly IStringLocalizer<OrphansController> _localizer;
        private readonly ServerDataDownLoader<ChildrenHouseDTO> _downLoader;
        private readonly IURLChildrenHouseBuilder _URLChildrenHouseBuilder;
        private readonly ServerDataDownLoader<AddressDTO> _addressDownLoader;
        private readonly IURLAddressBuilder _URLAddressBuilder;
        private readonly string _apiPath = "api/v1/childrenHouse";
        private readonly string _apiAddressPath = "api/v1/address";
        private readonly IFileStreamCreater _streamCreater;

        public OrphanagesController(IUnitOfWorkAsync unitOfWork,
                                IStringLocalizer<OrphansController> localizer,
                                ServerDataDownLoader<ChildrenHouseDTO> downLoader,
                                IURLChildrenHouseBuilder URLChildrenHouseBuilder,
                                IFileStreamCreater streamCreater,
                                IURLAddressBuilder URLAddressBuilder,
                                ServerDataDownLoader<AddressDTO> addressDownLoader)
           : base(unitOfWork)
        {
            _localizer = localizer;
            _downLoader = downLoader;
            _URLChildrenHouseBuilder = URLChildrenHouseBuilder;
            _streamCreater = streamCreater;
            _URLAddressBuilder = URLAddressBuilder;
            _addressDownLoader = addressDownLoader;
        }

        #endregion

        #region ActionMethods


        [AllowAnonymous]
        public async Task<IActionResult> Index(OrphanageSearchModel searchModel, SortStateOrphanages sortOrder = SortStateOrphanages.NameAsc)
        {
            var url = _URLChildrenHouseBuilder.GetAllWithFilter(_apiPath, searchModel, sortOrder);
            IEnumerable<ChildrenHouseDTO> childrenHouse = null;

            try
            {
                childrenHouse = await _downLoader.GetAllAsync(url, HttpContext.Session);
            }
            catch (ArgumentNullException)
            {
                return Redirect("/Home/Error");
            }
            catch (HttpRequestException)
            {
                return Redirect("/Home/Error");
            }
            catch (JsonException)
            {
                return Redirect("/Home/Error");
            }

            var orphanages = childrenHouse.Select(house => new Orphanage()
            {
                ID = house.ID,
                Name = house.Name,
                AdressID = house.AdressID,
                LocationID = house.LocationID,
                Rating = house.Rating,
                Avatar = house.PhotoPath,
                Adress = GetAddress(house.AdressID.Value).Result
            }); 
           
            
            GetViewData();

            return View(orphanages);
        }

        private async Task<Address> GetAddress(int id)
        {
            var url = _URLAddressBuilder.GetById(_apiAddressPath, id);

            var address =  await _addressDownLoader.GetByIdAsync(url, HttpContext.Session);

            var newAddress = new Address()
            {
                ID = address.ID,
                Country =  address.Country,
                Region = address.Region,
                City = address.City,
                Street = address.Street,
                House = address.House
            };

            return newAddress;
        }

        // GET: Orphanages/Details/5
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // POST: Orphanages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Adress,Rating,Avatar")] Orphanage orphanage,
            IFormFile file) //TODO: AlPa -> Research Bind To Annotations
        {
            await ImageHelper.SetAvatar(orphanage, file, "wwwroot\\avatars");

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

            if (ModelState.IsValid)
            {
                await _unitOfWorkAsync.Orphanages.Create(orphanage);
                await _unitOfWorkAsync.Orphanages.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            GetViewData();

            return View(orphanage);
        }

        // GET: Orphanages/Edit/5
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

        [AllowAnonymous]
        public IActionResult SearchByTypeHelp()
        {
            GetViewData();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult SearchResult(string typeHelp)
        {
            ViewData["TypeHelp"] = typeHelp;
            IEnumerable<Orphanage> list = new List<Orphanage>();
            if(typeHelp != null)
            list = _unitOfWorkAsync.Orphanages.Get(
                orp => orp.Donations.Where(
                    donat => donat.DonationItem.TypeBaseItem.Where(
                        donatitem =>  donatitem.Type.Name.ToLower().Contains(typeHelp.ToLower())).
                        Count() > 0 && donat.IsRequest).
                    Count() > 0);
            GetViewData();

            return View("SearchResult", list);
        }

        [AllowAnonymous]
        public IActionResult SearchOrphanageOnMap()
        {
            var orphanages = _unitOfWorkAsync.Orphanages.GetForSearchOrphanageOnMap();
            GetViewData();

            return View(orphanages);
        }


        #endregion

        #region Private Helpers

        private bool Contains(Address addr)
        {
            foreach (var word in _searchModel.Address.Split())
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

        private IQueryable<Orphanage> GetSorted(IQueryable<Orphanage> orphanages, SortStateOrphanages sortOrder)
        {
            ViewData["NameSort"] = sortOrder == SortStateOrphanages.NameAsc
                ? SortStateOrphanages.NameDesc : SortStateOrphanages.NameAsc;
            ViewData["AddressSort"] = sortOrder == SortStateOrphanages.AddressAsc
                ? SortStateOrphanages.AddressDesc : SortStateOrphanages.AddressAsc;
            ViewData["RatingSort"] = sortOrder == SortStateOrphanages.RatingAsc
                ? SortStateOrphanages.RatingDesc : SortStateOrphanages.RatingAsc;

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

        private IQueryable<Orphanage> GetFiltered(IQueryable<Orphanage> orphanages,
            OrphanageSearchModel searchModel)
        {
            if (searchModel != null)
            {
                _searchModel = searchModel;

                if (!string.IsNullOrEmpty(searchModel.Name))
                    orphanages = orphanages.Where(x => x.Name.Contains(searchModel.Name));

                if (!string.IsNullOrEmpty(searchModel.Address))
                    orphanages = orphanages.Where(x => Contains(x.Adress));

                if (searchModel.Rating > 0)
                    orphanages = orphanages.Where(x => x.Rating >= searchModel.Rating);
            }
            GetViewData();

            return orphanages;
        }

        private bool GetCoordProp(Address address, out Tuple<float?, float?> result)
        {
            result = null;
            bool forOut = false;

            var nominatim = new Nominatim.API.Geocoders.ForwardGeocoder();
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

