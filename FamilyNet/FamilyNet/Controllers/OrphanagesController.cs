using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models;
using System.IO;
using FamilyNet.Models.ViewModels;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using FamilyNet.Downloader;
using DataTransferObjects;
using FamilyNet.StreamCreater;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using FamilyNet.IdentityHelpers;

namespace FamilyNet.Controllers
{
    public class OrphanagesController : Controller
    {
        #region Private fields

        private readonly IIdentityInformationExtractor _identityInformationExtactor;
        private readonly IStringLocalizer<OrphansController> _localizer;
        private readonly ServerChildrenHouseDownloader _childrenHouseDownloader;
        private readonly ServerAddressDownloader _addressDownLoader;
        private readonly ServerLocationDownloader _locationDownLoader;
        private readonly IURLChildrenHouseBuilder _URLChildrenHouseBuilder;
        private readonly IURLAddressBuilder _URLAddressBuilder;
        private readonly IURLLocationBuilder _URLLocationBuilder;
        private readonly string _apiPath = "api/v1/childrenHouse";
        private readonly string _apiAddressPath = "api/v1/address";
        private readonly string _apiLocationPath = "api/v1/location";
        private readonly IFileStreamCreater _streamCreater;

        private readonly IURLDonationsBuilder _URLDonation;
        private readonly ServerSimpleDataDownloader<DonationDetailDTO> _donation;
        private readonly string _apiDonationPath = "api/v1/donations";

        #endregion

        #region Ctor

        public OrphanagesController(IStringLocalizer<OrphansController> localizer,
                                ServerChildrenHouseDownloader downLoader,
                                IURLChildrenHouseBuilder URLChildrenHouseBuilder,
                                IFileStreamCreater streamCreater,
                                IURLAddressBuilder URLAddressBuilder,
                                ServerAddressDownloader addressDownLoader,
                                ServerLocationDownloader locationDownLoader,
                                IURLLocationBuilder URLLocationBuilder,
                                IURLDonationsBuilder URLDonation,
                                ServerSimpleDataDownloader<DonationDetailDTO> donation,
                                IIdentityInformationExtractor identityInformationExtactor)
        {
            _localizer = localizer;
            _streamCreater = streamCreater;
            _URLChildrenHouseBuilder = URLChildrenHouseBuilder;
            _URLAddressBuilder = URLAddressBuilder;
            _URLLocationBuilder = URLLocationBuilder;
            _childrenHouseDownloader = downLoader;
            _addressDownLoader = addressDownLoader;
            _locationDownLoader = locationDownLoader;
            _URLDonation = URLDonation;
            _donation = donation;
            _identityInformationExtactor = identityInformationExtactor;
        }

        #endregion

        #region ActionMethods

        public async Task<IActionResult> Index(OrphanageSearchModel searchModel, SortStateOrphanages sortOrder = SortStateOrphanages.NameAsc)
        {
            var url = _URLChildrenHouseBuilder.GetAllWithFilter(_apiPath, searchModel, sortOrder);
            IEnumerable<ChildrenHouseDTO> childrenHouse = null;

            try
            {
                childrenHouse = await _childrenHouseDownloader.GetAllAsync(url, HttpContext.Session);
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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLChildrenHouseBuilder.GetById(_apiPath, id.Value);
            ChildrenHouseDTO childrenHouseDTO = null;

            try
            {
                childrenHouseDTO = await _childrenHouseDownloader.GetByIdAsync(url, HttpContext.Session);
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

            if (childrenHouseDTO == null)
            {
                return NotFound();
            }

            var orphanage = new Orphanage()
            {

                ID = childrenHouseDTO.ID,
                Name = childrenHouseDTO.Name,
                AdressID = childrenHouseDTO.AdressID,
                LocationID = childrenHouseDTO.LocationID,
                Rating = childrenHouseDTO.Rating,
                Avatar = childrenHouseDTO.PhotoPath,
                Adress = GetAddress(childrenHouseDTO.AdressID.Value).Result
            };

            GetViewData();

            return View(orphanage);
        }


        public IActionResult Create()
        {
            GetViewData();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChildrenHouseCreateViewModel model)
        {

            var url = _URLAddressBuilder.CreatePost(_apiAddressPath);
            var msg = await _addressDownLoader.CreatePostAsync(url, model.Address, HttpContext.Session);

            if (msg.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (msg.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            Stream stream = null;

            if (model.ChildrenHouse.Avatar != null)
            {
                stream = _streamCreater.CopyFileToStream(model.ChildrenHouse.Avatar);
            }

            var addressDTO = msg.Content.ReadAsAsync<AddressDTO>().Result;
            model.ChildrenHouse.AdressID = addressDTO.ID;

            url = _URLLocationBuilder.CreatePost(_apiLocationPath);
            msg = await _locationDownLoader.ÑreatePostAsync(url, model.Address, HttpContext.Session);
            if (msg.StatusCode == HttpStatusCode.Created)
            {
                var locationDTO = msg.Content.ReadAsAsync<LocationDTO>().Result;
                model.ChildrenHouse.LocationID = locationDTO.ID;
            }

            url = _URLChildrenHouseBuilder.CreatePost(_apiPath);
            var status = await _childrenHouseDownloader.CreatePostAsync(url, model.ChildrenHouse,
                                                                stream,
                                                                String.Empty,
                                                                HttpContext.Session);
            if (msg.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (status != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            GetViewData();

            return Redirect("/Orphanages/Index");
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLChildrenHouseBuilder.GetById(_apiPath, id.Value);
            ChildrenHouseDTO childrenHouseDTO = null;
            AddressDTO addressDTO = null;
            try
            {
                childrenHouseDTO = await _childrenHouseDownloader.GetByIdAsync(url, HttpContext.Session);
                url = _URLAddressBuilder.GetById(_apiAddressPath, childrenHouseDTO.AdressID.Value);
                addressDTO = await _addressDownLoader.GetByIdAsync(url, HttpContext.Session);
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

            GetViewData();

            ChildrenHouseCreateViewModel model = new ChildrenHouseCreateViewModel
            {
                ChildrenHouse = childrenHouseDTO,
                Address = addressDTO
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChildrenHouseCreateViewModel model) //TODO: Check change id position
        {
            if (id != model.ChildrenHouse.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Stream stream = null;

            if (model.ChildrenHouse.Avatar != null)
            {
                stream = _streamCreater.CopyFileToStream(model.ChildrenHouse.Avatar);
            }

            var url = _URLAddressBuilder.GetById(_apiAddressPath, model.ChildrenHouse.AdressID.Value);

            if (model.ChildrenHouse.AdressID != null)
            {
                var msg = await _addressDownLoader.CreatePutAsync(url, model.Address, HttpContext.Session);

                if (msg.StatusCode == HttpStatusCode.NoContent)
                {
                    if (model.ChildrenHouse.LocationID != null)
                    {
                        url = _URLLocationBuilder.GetById(_apiLocationPath, model.ChildrenHouse.LocationID.Value);
                        msg = await _locationDownLoader.ÑreatePutAsync(url, model.Address, HttpContext.Session);
                    }
                    else
                    {
                        url = _URLLocationBuilder.CreatePost(_apiLocationPath);
                        msg = await _locationDownLoader.ÑreatePostAsync(url, model.Address, HttpContext.Session);
                        if (msg.StatusCode == HttpStatusCode.Created)
                        {
                            var locationDTO = msg.Content.ReadAsAsync<LocationDTO>().Result;
                            model.ChildrenHouse.LocationID = locationDTO.ID;
                        }
                    }
                }
            }


            url = _URLChildrenHouseBuilder.GetById(_apiPath, id);
            var status = await _childrenHouseDownloader.CreatePutAsync(url, model.ChildrenHouse,
                                                            stream, model.ChildrenHouse.Avatar?.FileName,
                                                            HttpContext.Session);
            if (status == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (status != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            GetViewData();

            return View(model);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLChildrenHouseBuilder.GetById(_apiPath, id.Value);
            ChildrenHouseDTO childrenHouseDTO = null;

            try
            {
                childrenHouseDTO = await _childrenHouseDownloader.GetByIdAsync(url, HttpContext.Session);
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

            if (childrenHouseDTO == null)
            {
                return NotFound();
            }

            var orphanage = new Orphanage()
            {

                ID = childrenHouseDTO.ID,
                Name = childrenHouseDTO.Name,
                AdressID = childrenHouseDTO.AdressID,
                LocationID = childrenHouseDTO.LocationID,
                Rating = childrenHouseDTO.Rating,
                Avatar = childrenHouseDTO.PhotoPath,
                Adress = GetAddress(childrenHouseDTO.AdressID.Value).Result
            };

            GetViewData();

            return View(orphanage);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var url = _URLChildrenHouseBuilder.GetById(_apiPath, id);
            ChildrenHouseDTO childrenHouseDTO = null;
            try
            {
                childrenHouseDTO = await _childrenHouseDownloader.GetByIdAsync(url, HttpContext.Session);
            }
            catch (ArgumentNullException)
            {
                return Redirect("/Home/Error");
            }
            catch (HttpRequestException)
            {
                return Redirect("/Home/Error");
            }

            url = _URLAddressBuilder.GetById(_apiAddressPath, childrenHouseDTO.AdressID.Value);
            var addressStatus = await _addressDownLoader.DeleteAsync(url, HttpContext.Session);
            if (addressStatus != HttpStatusCode.OK)
            {
                return Redirect("/Home/Error");
            }

            if (childrenHouseDTO.LocationID != null)
            {
                url = _URLLocationBuilder.GetById(_apiLocationPath, childrenHouseDTO.LocationID.Value);
                var locationStatus = await _locationDownLoader.DeleteAsync(url, HttpContext.Session);
                if (locationStatus != HttpStatusCode.OK)
                {
                    return Redirect("/Home/Error");
                }
            }

            url = _URLChildrenHouseBuilder.GetById(_apiPath, id);
            var houseStatus = await _childrenHouseDownloader.DeleteAsync(url, HttpContext.Session);

            if (houseStatus == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }
            if (houseStatus != HttpStatusCode.OK)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult SearchByTypeHelp()
        {
            GetViewData();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchResult(string typeHelp)
        {
            ViewData["TypeHelp"] = typeHelp;
            IEnumerable<DonationDetailDTO> listDonation = null;
            if(typeHelp != null)
            {
                var url = _URLDonation.GetAllWithFilter(_apiDonationPath, typeHelp);
                listDonation =await _donation.GetAllAsync(url, HttpContext.Session);              
            }

            GetViewData();
            return View("SearchResult", listDonation);
        }

        public async Task<IActionResult> SearchOrphanageOnMap()
        {
            var url = _URLChildrenHouseBuilder.CreatePost(_apiPath);
            var houses = await _childrenHouseDownloader.GetAllAsync(url, HttpContext.Session);
            var filtredHouses = houses.Where(loc => loc.LocationID != null && loc.AdressID != null)
                .Select(orph =>
               new Orphanage
               {
                   Adress = GetAddress(orph.AdressID.Value).Result,
                   Name = orph.Name,
                   Location = GetLocation(orph.LocationID.Value).Result
               });

            GetViewData();

            return View(filtredHouses);
        }


        #endregion

        #region Private Helpers

        private async Task<Address> GetAddress(int id)
        {
            var url = _URLAddressBuilder.GetById(_apiAddressPath, id);

            var address = await _addressDownLoader.GetByIdAsync(url, HttpContext.Session);

            var newAddress = new Address()
            {
                ID = address.ID,
                Country = address.Country,
                Region = address.Region,
                City = address.City,
                Street = address.Street,
                House = address.House
            };

            GetViewData();

            return newAddress;
        }

        private async Task<Location> GetLocation(int id)
        {
            var url = _URLLocationBuilder.GetById(_apiLocationPath, id);

            var location = await _locationDownLoader.GetByIdAsync(url, HttpContext.Session);

            var newLocation = new Location()
            {
                ID = location.ID,
                MapCoordX = location.MapCoordX,
                MapCoordY = location.MapCoordY
            };

            GetViewData();

            return newLocation;
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

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                                 ViewData);
        }

        #endregion
    }
}

