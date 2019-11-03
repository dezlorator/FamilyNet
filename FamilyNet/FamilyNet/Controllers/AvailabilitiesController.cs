using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DataTransferObjects;
using FamilyNet.Downloader;
using Microsoft.AspNetCore.Mvc;

namespace FamilyNet.Controllers
{
    public class AvailabilitiesController : Controller
    {
        private readonly IServerAvailabilitiesDownloader _availabilitiesDownLoader;
        private readonly IURLAvailabilitiesBuilder _URLAvailabilitiesBuilder;
        private readonly string _apiPath = "api/v1/availabilities";

        public AvailabilitiesController(IServerAvailabilitiesDownloader availabilitiesDownloader,
                                        IURLAvailabilitiesBuilder urlAvailabilitiesBuilder)
        {
            _availabilitiesDownLoader = availabilitiesDownloader;
            _URLAvailabilitiesBuilder = urlAvailabilitiesBuilder;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
         public async Task<IActionResult> Create(AvailabilityDTO availabilityDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(availabilityDTO);
            }

            var url = _URLAvailabilitiesBuilder.CreatePost(_apiPath);
            var status = await _availabilitiesDownLoader.CreatePostAsync(url, availabilityDTO,
                                                 HttpContext.Session);

            if (status == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (status != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
            }

            return RedirectToAction(nameof(Index));
        }


    }
}