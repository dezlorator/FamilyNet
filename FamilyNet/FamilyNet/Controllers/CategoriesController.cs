using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Models.ViewModels;
using DataTransferObjects;
using FamilyNet.Downloader;
using Microsoft.Extensions.Localization;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

namespace FamilyNet.Controllers
{
    public class CategoriesController : BaseController
    {

        #region private fields

        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IStringLocalizer<CategoriesController> _localizer;
        private readonly ServerSimpleDataDownloader<CategoryDTO> _downloader;
        private readonly IURLCategoriesBuilder _URLBuilder;
        private readonly string _apiPath = "api/v1/categories";

        #endregion

        #region ctor

        public CategoriesController(IUnitOfWorkAsync unitOfWork,
                                 IStringLocalizer<CategoriesController> localizer,
                                 ServerSimpleDataDownloader<CategoryDTO> downloader,
                                 IURLCategoriesBuilder uRLBuilder)
            : base(unitOfWork)
        {
            _localizer = localizer;
            _downloader = downloader;
            _URLBuilder = uRLBuilder;
        }

        #endregion

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var url = _URLBuilder.GetAll(_apiPath);
            IEnumerable<CategoryDTO> categoryDTO = null;

            try
            {
                categoryDTO = await _downloader.GetAllAsync(url);
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

            var categories = categoryDTO.Select(category => new BaseItemType()
            {
                ID = category.ID,
                Name = category.Name
            });

            return View(categories);
        }

        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Create()
        {
            await Check();

            var donationItemsList = _unitOfWorkAsync.DonationItems.GetAll().ToList();
            ViewBag.ListOfDonationItems = donationItemsList;

            var orphanagesList = _unitOfWorkAsync.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;

            GetViewData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Create(DonationViewModel model)
        {
            var url = _URLDonationItemsBuilder.CreatePost(_apiDonationItemsPath);
            var msg = await _downloaderItems.CreatePostAsync(url, model.DonationItem);

            if (msg.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            var itemDTO = msg.Content.ReadAsAsync<DonationItemDTO>().Result;
            model.Donation.DonationItemID = itemDTO.ID;

            url = _URLDonationsBuilder.CreatePost(_apiPath);
            msg = await _downloader.CreatePostAsync(url, model.Donation);

            if (msg.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            GetViewData();

            return Redirect("/Donations/Index");
        }

        private void GetViewData()
        {
            ViewData["CategoriesList"] = _localizer["CategoriesList"];
        }
    }
}