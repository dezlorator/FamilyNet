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
using System.IO;
using FamilyNet.StreamCreater;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class DonationsController : BaseController
    {
        #region private fields

        private readonly IStringLocalizer<DonationsController> _localizer;
        private readonly ServerSimpleDataDownloader<DonationDetailDTO> _downloader;
        private readonly ServerSimpleDataDownloader<CategoryDTO> _downloaderCategories;
        private readonly ServerSimpleDataDownloader<DonationItemDTO> _downloaderItems;
        private readonly IURLDonationsBuilder _URLDonationsBuilder;
        private readonly IURLDonationItemsBuilder _URLDonationItemsBuilder;
        private readonly string _apiPath = "api/v1/donations";
        private readonly IFileStreamCreater _streamCreater;
        private readonly string _apiCategoriesPath = "api/v1/categories";
        private readonly string _apiDonationItemsPath = "api/v1/donationItems/";

        #endregion

        #region ctor

        public DonationsController(IUnitOfWorkAsync unitOfWork,
                                 IStringLocalizer<DonationsController> localizer,
                                 ServerSimpleDataDownloader<DonationDetailDTO> downloader,
                                 ServerSimpleDataDownloader<CategoryDTO> downloaderCategories,
                                 ServerSimpleDataDownloader<DonationItemDTO> downloaderItems,
                                 IURLDonationsBuilder uRLDonationsBuilder,
                                 IURLDonationItemsBuilder uRLDonationItemsBuilder,
                                 IFileStreamCreater streamCreater)
            : base(unitOfWork)
        {
            _localizer = localizer;
            _downloader = downloader;
            _downloaderCategories = downloaderCategories;
            _downloaderItems = downloaderItems;
            _URLDonationsBuilder = uRLDonationsBuilder;
            _URLDonationItemsBuilder = uRLDonationItemsBuilder;
            _streamCreater = streamCreater;
        }

        #endregion

        [AllowAnonymous]
        public async Task<IActionResult> Index(int orphanageId)
        {
            var url = _URLDonationsBuilder.GetAllWithFilter(_apiPath,
                                                            orphanageId);
            IEnumerable<DonationDTO> donationDTO = null;

            try
            {
                donationDTO = await _downloader.GetAllAsync(url);
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

            var donations = donationDTO.Select(donation => new Donation()
            {
                ID = donation.ID,

                Orphanage = new Orphanage()
                {
                    Name = donation.OrphanageName,

                    Adress = new Address()
                    {
                        City = donation.City,
                    }
                },

                DonationItem = new DonationItem()
                {
                    Name = donation.ItemName,
                    Description = donation.ItemDescription,
                    TypeBaseItem = donation.Types.Select(async t => await GetTypeBaseItemsAsync(t))
                                                                          .Select(t => t.Result)
                                                                          .Where(i => i != null)
                                                                          .ToList()
                },

                LastDateWhenStatusChanged = donation.LastDateWhenStatusChanged,

                Status = (DonationStatus)Enum.Parse(typeof(DonationStatus), donation.Status, true)
            });

            GetViewData();

            return View(donations);
        }

        private async Task<TypeBaseItem> GetTypeBaseItemsAsync(int typeId)
        {
            var typeBaseItem = new TypeBaseItem()
            {
                Type = await GetTypeByIdAsync(typeId)
            };

            return typeBaseItem;
        }

        private async Task<BaseItemType> GetTypeByIdAsync(int id)
        {
            var url = _URLDonationsBuilder.GetById(_apiCategoriesPath, id);

            var category = await _downloaderCategories.GetByIdAsync(url);

            var newCategory = new BaseItemType()
            {
                ID = category.ID,
                Name = category.Name,
            };

            return newCategory;
        }

        [AllowAnonymous]
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
                donationDetailDTO = await _downloader.GetByIdAsync(url);
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

            var donation = new Donation()
            {
                Orphanage = new Orphanage()
                {
                    Name = donationDetailDTO.OrphanageName,
                    Rating = donationDetailDTO.OrphanageRating,

                    Adress = new Address()
                    {
                        City = donationDetailDTO.City,
                        Street = donationDetailDTO.OrphanageStreet,
                        House = donationDetailDTO.OrphanageHouse
                    }
                },

                DonationItem = new DonationItem()
                {
                    Name = donationDetailDTO.ItemName,
                    Description = donationDetailDTO.ItemDescription,
                    TypeBaseItem = donationDetailDTO.Types.Select(async t => await GetTypeBaseItemsAsync(t))
                                                                          .Select(t => t.Result)
                                                                          .Where(i => i != null)
                                                                          .ToList()
                }
            };

            GetViewData();

            return View(donation);
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
        public async Task<IActionResult> Create(DonationCreateViewModel model)
        {
            var url = _URLDonationItemsBuilder.CreatePost(_apiDonationItemsPath);
            var msg = await _downloaderItems.СreatePostAsync(url, model.DonationItem);

            if (msg.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            var itemDTO = msg.Content.ReadAsAsync<DonationItemDTO>().Result;
            model.Donation.DonationItemID = itemDTO.ID;

            url = _URLDonationsBuilder.CreatePost(_apiPath);
            msg = await _downloader.СreatePostAsync(url, model.Donation);

            if (msg.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            GetViewData();

            return Redirect("/Donations/Index");
        }

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

            var url = _URLDonationsBuilder.GetById(_apiPath, id.Value);
            DonationDTO donationDTO;

            try
            {
                donationDTO = await _downloader.GetByIdAsync(url);
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

            var baseItemsList = _unitOfWorkAsync.DonationItems.GetAll().ToList();
            ViewBag.ListOfBaseItems = baseItemsList;

            var orphanagesList = _unitOfWorkAsync.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;

            var categoriesList = _unitOfWorkAsync.BaseItemTypes.GetAll().ToList();
            ViewBag.ListOfCategories = categoriesList;

            GetViewData();

            return View(donationDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Edit(int id, DonationDetailDTO donationDTO)
        {
            if (id != donationDTO.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(donationDTO);
            }

            var url = _URLDonationsBuilder.GetById(_apiPath, id);
            var msg = await _downloader.СreatePutAsync(url, donationDTO);

            if (msg.StatusCode != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
            }

            return Redirect("/Donations/Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditStatus(int? id)
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

            var url = _URLDonationsBuilder.GetById(_apiPath, id.Value);
            DonationDTO donationDTO;

            try
            {
                donationDTO = await _downloader.GetByIdAsync(url);
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

            var donationsList = _unitOfWorkAsync.Donations.GetAll().ToList();
            ViewBag.ListOfDonations = donationsList;

            GetViewData();

            return View(donationDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditStatus(int id, DonationDetailDTO donationDTO)
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
            var msg = await _downloader.СreatePutAsync(url, donationDTO);

            if (msg.StatusCode != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
            }

            return Redirect("/Donations/Index");
        }

        [Authorize(Roles = "Admin")]
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
                donationDTO = await _downloader.GetByIdAsync(url);
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

            return View(id.Value);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var url = _URLDonationsBuilder.GetById(_apiPath, id);
            var msg = await _downloader.DeleteAsync(url);

            if (msg.StatusCode != HttpStatusCode.OK)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return Redirect("/ODonations/Index");
        }

        private void GetViewData()
        {
            ViewData["DonationsList"] = _localizer["DonationsList"];
        }
    }
}
