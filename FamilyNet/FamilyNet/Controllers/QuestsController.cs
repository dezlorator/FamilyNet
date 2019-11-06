using System;
using System.Linq;
using System.Collections.Generic;
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
using System.IO;
using FamilyNet.StreamCreater;
using FamilyNet.IdentityHelpers;

namespace FamilyNet.Controllers
{
    public class QuestsController : Controller
    {
        #region private fields

        private readonly IIdentityInformationExtractor _identityInformationExtactor;
        private readonly IStringLocalizer<QuestsController> _localizer;
        private readonly ServerSimpleDataDownloader<QuestDTO> _downloader;
        private readonly ServerSimpleDataDownloader<DonationItemDTO> _downloaderItems;
        private readonly ServerDataDownloader<ChildrenHouseDTO> _downloaderOrphanages;

        private readonly IURLQuestsBuilder _URLBuilder;
        private readonly IURLDonationsBuilder _URLDonationsBuilder;
        private readonly IURLDonationItemsBuilder _URLDonationItemsBuilder;
        private readonly IURLChildrenHouseBuilder _URLChildrenHouseBuilder;

        private readonly string _apiPath = "api/v1/quests";
        private readonly string _apiCategoriesPath = "api/v1/categories";
        private readonly string _apiDonationItemsPath = "api/v1/donationItems";
        private readonly string _apiOrphanagesPath = "api/v1/childrenHouse";

        #endregion
        public QuestsController(IStringLocalizer<QuestsController> localizer,
                                 ServerSimpleDataDownloader<QuestDTO> downloader,
                                 ServerSimpleDataDownloader<DonationItemDTO> downloaderItems,
                                 ServerDataDownloader<ChildrenHouseDTO> serverChildrenHouseDownloader,
                                 IURLQuestsBuilder uRLQuestsBuilder,
                                 IURLDonationsBuilder uRLDonationsBuilder,
                                 IURLDonationItemsBuilder uRLDonationItemsBuilder,
                                 IURLChildrenHouseBuilder uRLChildrenHouseBuilder,
                                 IIdentityInformationExtractor identityInformationExtactor)
        {
            _localizer = localizer;
            _downloader = downloader;
            _downloaderItems = downloaderItems;
            _downloaderOrphanages = serverChildrenHouseDownloader;
            _URLBuilder = uRLQuestsBuilder;
            _URLDonationsBuilder = uRLDonationsBuilder;
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

        private void GetViewData()
        {
            ViewData["QuestsList"] = _localizer["QuestsList"];

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                                 ViewData);
        }
    }
}
