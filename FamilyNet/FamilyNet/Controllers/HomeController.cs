using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Hosting;

namespace FamilyNet.Controllers {
    public class HomeController : BaseController {
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IUnitOfWorkAsync unitOfWork, IHostingEnvironment environment, IStringLocalizer<HomeController> localizer) : base(unitOfWork) {
            _localizer = localizer;
            _hostingEnvironment = environment;

        }

        public async Task<IActionResult> Index() {
            ViewData["Best"] = _unitOfWorkAsync.Orphanages.GetAll()
              .OrderByDescending(c => c.Rating)

              .Take(3);

            GetViewData();
            return View();
        }
        public IActionResult Privacy() {
            GetViewData();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            GetViewData();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void GetViewData() {
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
