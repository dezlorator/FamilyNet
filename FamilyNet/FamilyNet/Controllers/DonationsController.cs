using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using DataTransferObjects;
using FamilyNet.Downloader;
using Microsoft.Extensions.Localization;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using FamilyNet.IdentityHelpers;
using FamilyNet.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace FamilyNet.Controllers
{
    public class DonationsController : Controller
    {
        #region private fields

        private readonly IStringLocalizer<DonationsController> _localizer;
        private readonly ServerSimpleDataDownloader<DonationDetailDTO> _downloader;
        private readonly ServerSimpleDataDownloader<CategoryDTO> _downloaderCategories;
        private readonly ServerSimpleDataDownloader<DonationItemDTO> _downloaderItems;
        private readonly ServerDataDownloader<ChildrenHouseDTO> _downloaderOrphanages;
        private readonly IURLDonationsBuilder _URLDonationsBuilder;
        private readonly IURLDonationItemsBuilder _URLDonationItemsBuilder;
        private readonly IURLChildrenHouseBuilder _URLChildrenHouseBuilder;
        private readonly IURLCategoriesBuilder _URLCategoriesBuilder;
        private readonly IIdentityInformationExtractor _identityInformationExtactor;

        private readonly string _apiPath = "api/v1/donations";
        private readonly string _apiCategoriesPath = "api/v1/categories";
        private readonly string _apiDonationItemsPath = "api/v1/donationItems";
        private readonly string _apiOrphanagesPath = "api/v1/childrenHouse";

        #endregion

        #region ctor

        public DonationsController(IStringLocalizer<DonationsController> localizer,
                                 ServerSimpleDataDownloader<DonationDetailDTO> downloader,
                                 ServerSimpleDataDownloader<CategoryDTO> downloaderCategories,
                                 ServerSimpleDataDownloader<DonationItemDTO> downloaderItems,
                                 ServerDataDownloader<ChildrenHouseDTO> serverChildrenHouseDownloader,
                                 IURLDonationsBuilder uRLDonationsBuilder,
                                 IURLDonationItemsBuilder uRLDonationItemsBuilder,
                                 IURLChildrenHouseBuilder uRLChildrenHouseBuilder,
                                 IIdentityInformationExtractor identityInformationExtactor,
                                 IURLCategoriesBuilder uRLCategoriesBuilder)
        {
            _localizer = localizer;
            _downloader = downloader;
            _downloaderCategories = downloaderCategories;
            _downloaderItems = downloaderItems;
            _downloaderOrphanages = serverChildrenHouseDownloader;
            _URLDonationsBuilder = uRLDonationsBuilder;
            _URLDonationItemsBuilder = uRLDonationItemsBuilder;
            _URLChildrenHouseBuilder = uRLChildrenHouseBuilder;
            _identityInformationExtactor = identityInformationExtactor;
            _URLCategoriesBuilder = uRLCategoriesBuilder;
        }

        #endregion

        public async Task<IActionResult> Index(string forSearch)
        {
            var url = _URLDonationsBuilder.GetAllWithFilter(_apiPath,
                                                            forSearch);
            IEnumerable<DonationDTO> donationsDTO;

            try
            {
                donationsDTO = await _downloader.GetAllAsync(url, HttpContext.Session);
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

            return View(donationsDTO);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLDonationsBuilder.GetById(_apiPath, id.Value);
            DonationDetailDTO donationDetailDTO = null;

            try
            {
                donationDetailDTO = await _downloader.GetByIdAsync(url, HttpContext.Session);
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

            if (donationDetailDTO == null)
            {
                return NotFound();
            }

            GetViewData();

            return View(donationDetailDTO);
        }

        public async Task<IActionResult> Create()
        {
            var urlOrphanages = _URLChildrenHouseBuilder.GetAllWithFilter(_apiOrphanagesPath, new OrphanageSearchModel(), SortStateOrphanages.NameAsc);
            var orphanagesList = await _downloaderOrphanages.GetAllAsync(urlOrphanages, HttpContext.Session);
            ViewBag.ListOfOrphanages = orphanagesList;

            var urlCategories = _URLCategoriesBuilder.GetAll(_apiCategoriesPath);
            var categoriesList = await _downloaderCategories.GetAllAsync(urlCategories, HttpContext.Session);
            ViewBag.ListOfCategories = categoriesList;

            GetViewData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonationViewModel model)
        {
            var url = _URLDonationItemsBuilder.CreatePost(_apiDonationItemsPath);
            var msg = await _downloaderItems.CreatePostAsync(url, model.DonationItem, HttpContext.Session);

            if (msg.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (msg.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            var itemDTO = msg.Content.ReadAsAsync<DonationItemDTO>().Result;
            model.Donation.DonationItemID = itemDTO.ID;
          
            url = _URLDonationsBuilder.CreatePost(_apiPath);
            msg = await _downloader.CreatePostAsync(url, model.Donation, HttpContext.Session);

            if (msg.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (msg.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            GetViewData();

            return Redirect("/Donations/Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urlDonation = _URLDonationsBuilder.GetById(_apiPath, id.Value);

            DonationItemDTO item = null;
            DonationDetailDTO donation;

            try
            {
                donation = await _downloader.GetByIdAsync(urlDonation, HttpContext.Session);

                if (donation.DonationItemID != null)
                {
                    var urlItem = _URLDonationItemsBuilder.GetById(_apiDonationItemsPath, donation.DonationItemID.Value);
                    item = await _downloaderItems.GetByIdAsync(urlItem, HttpContext.Session);
                }
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

            var urlOrphanage = _URLChildrenHouseBuilder.GetAllWithFilter(_apiOrphanagesPath,
                                  new OrphanageSearchModel(), SortStateOrphanages.NameAsc);
            var orphanagesList = await _downloaderOrphanages.GetAllAsync(urlOrphanage, HttpContext.Session);
            ViewBag.ListOfOrphanages = orphanagesList;

            GetViewData();

            var model = new DonationViewModel
            {
                Donation = donation,
                DonationItem = item
            };

            GetViewData();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, DonationViewModel model)
        {
            if (id != model.Donation.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var url = _URLDonationsBuilder.GetById(_apiPath, id);
            var msg = await _downloader.CreatePutAsync(url, model.Donation, HttpContext.Session);

            if (msg.StatusCode != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            if (model.Donation.DonationItemID != null)
            {
                url = _URLDonationItemsBuilder.GetById(_apiDonationItemsPath, model.Donation.DonationItemID.Value);
                msg = await _downloaderItems.CreatePutAsync(url, model.DonationItem, HttpContext.Session);
            }

            if (msg.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (msg.StatusCode != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            GetViewData();

            return Redirect("/Donations/Index");
        }

        public async Task<IActionResult> EditStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLDonationsBuilder.GetById(_apiPath, id.Value);
            DonationDTO donationDTO;

            try
            {
                donationDTO = await _downloader.GetByIdAsync(url, HttpContext.Session);
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

            return View(donationDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(int id, DonationDetailDTO donationDTO) //TODO: completely remake
        {
            if (id != donationDTO.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(donationDTO.Status);
            }

            var url = _URLDonationsBuilder.GetById(_apiPath, id);
            var msg = await _downloader.CreatePutAsync(url, donationDTO, HttpContext.Session);

            if (msg.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (msg.StatusCode != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return Redirect("/Donations/Index");
        }

        public async Task<IActionResult> Donate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urlDonation = _URLDonationsBuilder.GetById(_apiPath, id.Value);

            var model = new DonateViewModel
            {
                Donation = await _downloader.GetByIdAsync(urlDonation, HttpContext.Session)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Donate(int id, DonateViewModel model)
        {
            if (id != model.Donation.ID)
            {
                return NotFound(); 
            }

            var role = HttpContext.Session.GetString("roles");
            var personId = HttpContext.Session.GetString("personId");

            if (personId == String.Empty || personId == null)
            {
                return Redirect("/Account/Login");
            }

            if(!int.TryParse(personId, out var charityMakerID))
            {
                return Redirect("/Home/Error");
            }

            var url = _URLDonationsBuilder.GetById(_apiPath, model.Donation.ID);
            model.Donation = await _downloader.GetByIdAsync(url, HttpContext.Session);
            model.Donation.CharityMakerID = charityMakerID;

            var msg = await _downloader.CreatePutAsync(url, model.Donation, HttpContext.Session);

            if (msg.StatusCode == HttpStatusCode.Unauthorized)
            {  
                return Redirect("/Account/Login");
            }

            if (msg.StatusCode != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
            }

            if (model.NeedHelp)
            {
                TempData["DonationID"] = model.Donation.ID;
                return Redirect("/Quests/Create");
            }

            GetViewData();

            return Redirect("/Donations/Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLDonationsBuilder.GetById(_apiPath, id.Value);
            DonationDTO donationDTO;

            try
            {
                donationDTO = await _downloader.GetByIdAsync(url, HttpContext.Session);
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

            if (donationDTO == null)
            {
                return NotFound();
            }

            GetViewData();

            return View(donationDTO);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var url = _URLDonationsBuilder.GetById(_apiPath, id);
            var msg = await _downloader.DeleteAsync(url, HttpContext.Session);

            if (msg.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (msg.StatusCode != HttpStatusCode.OK)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return Redirect("/Donations/Index");
        }

        private void GetViewData()
        {
            ViewData["DonationsList"] = _localizer["DonationsList"];
            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);
        }
    }
}