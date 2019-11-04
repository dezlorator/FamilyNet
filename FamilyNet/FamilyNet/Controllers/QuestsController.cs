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


        private readonly IURLQuestsBuilder _URLQuestsBuilder;
        private readonly IURLDonationsBuilder _URLDonationsBuilder;
        private readonly IURLDonationItemsBuilder _URLDonationItemsBuilder;
        private readonly IURLChildrenHouseBuilder _URLChildrenHouseBuilder;

        private readonly string _apiPath = "api/v1/donations";
        private readonly string _apiCategoriesPath = "api/v1/categories";
        private readonly string _apiDonationItemsPath = "api/v1/donationItems";
        private readonly string _apiOrphanagesPath = "api/v1/childrenHouse";

        #endregion

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
