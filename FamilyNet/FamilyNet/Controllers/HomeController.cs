using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Hosting;
using FamilyNet.Downloader;
using System.Collections.Generic;
using FamilyNet.Models.ViewModels;
using DataTransferObjects;
using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace FamilyNet.Controllers
{
    public class HomeController : Controller
    {
        #region fields

        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ServerChildrenHouseDownloader _childrenHouseDownloader;
        private readonly ServerAddressDownloader _addressDownLoader;
        private readonly IURLChildrenHouseBuilder _URLChildrenHouseBuilder;
        private readonly IURLAddressBuilder _URLAddressBuilder;
        private readonly string _apiPath = "api/v1/childrenHouse";
        private readonly string _apiAddressPath = "api/v1/address";

        #endregion

        #region ctor

        public HomeController(IHostingEnvironment environment,
                              IStringLocalizer<HomeController> localizer,
                              ServerChildrenHouseDownloader childrenHouseDownloader,
                              IURLChildrenHouseBuilder URLChildrenHouseBuilder,
                              ServerChildrenHouseDownloader downLoader,
                              IURLAddressBuilder URLAddressBuilder,
                              ServerAddressDownloader addressDownLoader)
        {
            _localizer = localizer;
            _hostingEnvironment = environment;
            _childrenHouseDownloader = childrenHouseDownloader;
            _URLChildrenHouseBuilder = URLChildrenHouseBuilder;
            _URLAddressBuilder = URLAddressBuilder;
            _addressDownLoader = addressDownLoader;
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            var search = new OrphanageSearchModel()
            {
                Page = 1,
                RowsCount = 3
            };

            var url = _URLChildrenHouseBuilder.GetAllWithFilter(_apiPath, search,
                                                                SortStateOrphanages.NameAsc);
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
                Adress = GetAddress(house.ID).Result
            });

            ViewData["Best"] = orphanages;
            GetViewData();

            return View();
        }

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

            return newAddress;
        }

        public IActionResult Privacy()
        {
            GetViewData();
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            GetViewData();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void GetViewData()
        {
            ViewData["CharityMakers"] = _localizer["CharityMakers"];
            ViewData["Children"] = _localizer["Children"];
            ViewData["Orphanages"] = _localizer["Orphanages"];
            ViewData["Representatives"] = _localizer["Representatives"];
            ViewData["SearchHelp"] = _localizer["SearchHelp"];
            ViewData["SingIn"] = _localizer["SingIn"];
            ViewData["Registration"] = _localizer["Registration"];
            ViewData["Volunteers"] = _localizer["Volunteers"];
            ViewData["FirstSlideComment"] = _localizer["FirstSlideComment"];
            ViewData["Description1"] = _localizer["Description1"];
            ViewData["SecondSlideComment"] = _localizer["SecondSlideComment"];
            ViewData["Description2"] = _localizer["Description2"];
            ViewData["ThirdSlideComment"] = _localizer["ThirdSlideComment"];
            ViewData["Description3"] = _localizer["Description3"];
        }
    }
}
