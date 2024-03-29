﻿using System;
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
    public class QuestsController : Controller
    {
        #region private fields

        private readonly IIdentityInformationExtractor _identityInformationExtactor;
        private readonly IStringLocalizer<QuestsController> _localizer;
        private readonly ServerSimpleDataDownloader<QuestDTO> _downloader;
        private readonly ServerSimpleDataDownloader<DonationDetailDTO> _downloaderDonations;
        private readonly ServerDataDownloader<ChildrenHouseDTO> _downloaderOrphanages;

        private readonly IURLQuestsBuilder _URLBuilder;
        private readonly IURLDonationsBuilder _URLDonationBuilder;
        private readonly IURLDonationItemsBuilder _URLDonationItemsBuilder;
        private readonly IURLChildrenHouseBuilder _URLChildrenHouseBuilder;

        private readonly string _apiPath = "api/v1/quests";
        private readonly string _apiDonationPath = "api/v1/donations";
        private readonly string _apiOrphanagesPath = "api/v1/childrenHouse";

        #endregion
        public QuestsController(IStringLocalizer<QuestsController> localizer,
                                 ServerSimpleDataDownloader<QuestDTO> downloader,
                                 ServerSimpleDataDownloader<DonationDetailDTO> downloaderDonations,
                                 ServerDataDownloader<ChildrenHouseDTO> serverChildrenHouseDownloader,
                                 IURLQuestsBuilder uRLQuestsBuilder,
                                 IURLDonationsBuilder uRLDonationsBuilder,
                                 IURLDonationItemsBuilder uRLDonationItemsBuilder,
                                 IURLChildrenHouseBuilder uRLChildrenHouseBuilder,
                                 IIdentityInformationExtractor identityInformationExtactor)
        {
            _localizer = localizer;
            _downloader = downloader;
            _downloaderDonations = downloaderDonations;
            _downloaderOrphanages = serverChildrenHouseDownloader;
            _URLBuilder = uRLQuestsBuilder;
            _URLDonationBuilder = uRLDonationsBuilder;
            _URLDonationItemsBuilder = uRLDonationItemsBuilder;
            _URLChildrenHouseBuilder = uRLChildrenHouseBuilder;
            _identityInformationExtactor = identityInformationExtactor;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(string forSearch, string status)
        {
            var url = _URLBuilder.GetAllWithFilter(_apiPath,
                                                   forSearch, status);

            IEnumerable<QuestDTO> questsDTO;

            try
            {
                questsDTO = await _downloader.GetAllAsync(url, HttpContext.Session);
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

            return View(questsDTO);
        }

        public IActionResult Create()
        {
            GetViewData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestDTO quest)
        {
            quest.DonationID = (int)TempData["DonationID"];
            var url = _URLBuilder.CreatePost(_apiPath);
            var msg = await _downloader.CreatePostAsync(url, quest, HttpContext.Session);

            if (msg.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (msg.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return Redirect("/Quests/Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLBuilder.GetById(_apiPath, id.Value);
            QuestDTO questDTO;

            try
            {
                questDTO = await _downloader.GetByIdAsync(url, HttpContext.Session);
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

            if (questDTO == null)
            {
                return NotFound();
            }

            GetViewData();

            return View(questDTO);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLBuilder.GetById(_apiPath, id.Value);

            QuestDTO quest;

            try
            {
                quest = await _downloader.GetByIdAsync(url, HttpContext.Session);
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

            return View(quest);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, QuestDTO quest)
        {
            if (id != quest.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(quest);
            }

            var url = _URLBuilder.GetById(_apiPath, id);
            var msg = await _downloader.CreatePutAsync(url, quest, HttpContext.Session);

            if (msg.StatusCode != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
            }

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

        public async Task<IActionResult> Take(int id)
        {
            var url = _URLBuilder.GetById(_apiPath, id);
            QuestDTO quest;

            try
            {
                quest = await _downloader.GetByIdAsync(url, HttpContext.Session);
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

            var personId = HttpContext.Session.GetString("personId");

            if (personId == String.Empty || personId == null)
            {
                return Redirect("/Account/Login");
            }

            if (!int.TryParse(personId, out var volunteerID))
            {
                return Redirect("/Home/Error");
            }

            quest.VolunteerID = volunteerID;

            var msg = await _downloader.CreatePutAsync(url, quest, HttpContext.Session);

            if (msg.StatusCode != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return Redirect("/Quests/Index");
        }

        public async Task<IActionResult> MarkAsDone(int id)
        {
            var url = _URLBuilder.GetById(_apiPath, id);
            QuestDTO quest;

            try
            {
                quest = await _downloader.GetByIdAsync(url, HttpContext.Session);
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

            quest.Status = "Done";
            var msg = await _downloader.CreatePutAsync(url, quest, HttpContext.Session);

            if (msg.StatusCode != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return Redirect("/Quests/Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLBuilder.GetById(_apiPath, id.Value);
            QuestDTO questDTO;

            try
            {
                questDTO = await _downloader.GetByIdAsync(url, HttpContext.Session);
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

            if (questDTO == null)
            {
                return NotFound();
            }

            GetViewData();

            return View(questDTO);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var url = _URLBuilder.GetById(_apiPath, id);
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

            return Redirect("/Quests/Index");
        }

        private void GetViewData()
        {
            ViewData["QuestsList"] = _localizer["QuestsList"];

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                                 ViewData);
        }
    }
}
