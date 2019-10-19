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
        private readonly ServerSimpleDataDownloader<DonationDetailDTO> _downLoader;
        private readonly IURLDonationsBuilder _URLDonationsBuilder;
        private readonly string _apiPath = "api/v1/donations";
        private readonly IFileStreamCreater _streamCreater;

        #endregion

        #region ctor

        public DonationsController(IUnitOfWorkAsync unitOfWork,
                                 IStringLocalizer<DonationsController> localizer,
                                 ServerSimpleDataDownloader<DonationDetailDTO> downLoader,
                                 IURLDonationsBuilder uRLDonationsBuilder,
                                 IFileStreamCreater streamCreater)
            : base(unitOfWork)
        {
            _localizer = localizer;
            _downLoader = downLoader;
            _URLDonationsBuilder = uRLDonationsBuilder;
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
                donationDTO = await _downLoader.GetAllAsync(url);
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
                CharityMakerID = donation.CharityMakerID,
                DonationItemID = donation.DonationItemID,
                OrphanageID = donation.OrphanageID
            });

            GetViewData();

            return View(donations);
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
                donationDetailDTO = await _downLoader.GetByIdAsync(url);
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
                        City = donationDetailDTO.OrphanageCity,
                        Street = donationDetailDTO.OrphanageStreet,
                        House = donationDetailDTO.OrphanageHouse
                    }
                },

                DonationItem = new DonationItem()
                {
                    Name = donationDetailDTO.ItemName,
                    Description = donationDetailDTO.ItemDescription
                }
            };

            GetViewData();

            return View(donation);
        }

        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Create()
        {
            await Check();

            var donationsList = new List<Donation>();
            donationsList = _unitOfWorkAsync.Donations.GetAll().ToList();
            ViewBag.ListOfDonations = donationsList;
            GetViewData();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Create([Bind("DonationItemID, CharityMakerID, OrphanageID")]
                                                DonationDetailDTO donationDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(donationDTO);
            }

            var url = _URLDonationsBuilder.CreatePost(_apiPath);
            var status = await _downLoader.СreatetePostAsync(url, donationDTO);

            if (status != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
            }

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
                donationDTO = await _downLoader.GetByIdAsync(url);
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
        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Edit(int id, DonationDetailDTO donationDTO)
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
            var status = await _downLoader.СreatePutAsync(url, donationDTO);

            if (status != HttpStatusCode.NoContent)
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
                donationDTO = await _downLoader.GetByIdAsync(url);
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
                return View(donationDTO);
            }

            var url = _URLDonationsBuilder.GetById(_apiPath, id);
            var status = await _downLoader.СreatePutAsync(url, donationDTO);

            if (status != HttpStatusCode.NoContent)
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
                donationDTO = await _downLoader.GetByIdAsync(url);
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
            var status = await _downLoader.DeleteAsync(url);

            if (status != HttpStatusCode.OK)
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
