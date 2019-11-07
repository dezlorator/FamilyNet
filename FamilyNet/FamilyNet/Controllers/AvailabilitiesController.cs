using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DataTransferObjects;
using FamilyNet.Downloader;
using FamilyNet.IdentityHelpers;
using FamilyNet.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FamilyNet.Controllers
{
    public class AvailabilitiesController : Controller
    {
        private readonly IIdentityInformationExtractor _identityInformationExtactor;
        private readonly IServerAvailabilitiesDownloader _availabilitiesDownLoader;
        private readonly IURLAvailabilitiesBuilder _URLAvailabilitiesBuilder;
        private readonly string _apiPath = "api/v1/availabilities";

        public AvailabilitiesController(IServerAvailabilitiesDownloader availabilitiesDownloader,
                                        IURLAvailabilitiesBuilder urlAvailabilitiesBuilder,
                                        IIdentityInformationExtractor identityInformationExtactor)
        {
            _availabilitiesDownLoader = availabilitiesDownloader;
            _URLAvailabilitiesBuilder = urlAvailabilitiesBuilder;
            _identityInformationExtactor = identityInformationExtactor;
        }

        public async Task<IActionResult> Index()
        {
            var url = _URLAvailabilitiesBuilder
                 .GetAll(_apiPath);

            IEnumerable<AvailabilityDTO> availabilitiesDTO = null;

            try
            {
                availabilitiesDTO = await _availabilitiesDownLoader.GetAllAsync(url, HttpContext.Session);
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

            var vm = new ScheduleViewModel
            {
                Days = availabilitiesDTO.Select(a => a.DayOfWeek).Distinct().OrderBy(d => d),
                Hours = availabilitiesDTO.Select(a => a.StartTime.TimeOfDay).Distinct().OrderBy(t => t),
                //Date = availabilitiesDTO.Select(a => a.FromHour),
                //AvailabilityDTOList = availabilitiesDTO.GroupBy(a => a.FromHour.TimeOfDay).OrderBy(g => g.Key)
                 //Sunday = availabilitiesDTO.Where(a => a.DayOfWeek == DayOfWeek.Sunday).OrderBy(a => a.FromHour.TimeOfDay),
                 //Monday = availabilitiesDTO.Where(a => a.DayOfWeek == DayOfWeek.Monday).OrderBy(a => a.FromHour.TimeOfDay),
                 //Tuesday = availabilitiesDTO.Where(a => a.DayOfWeek == DayOfWeek.Tuesday).OrderBy(a => a.FromHour.TimeOfDay),
                 //Wednesday = availabilitiesDTO.Where(a => a.DayOfWeek == DayOfWeek.Wednesday).OrderBy(a => a.FromHour.TimeOfDay),
                 //Thursday = availabilitiesDTO.Where(a => a.DayOfWeek == DayOfWeek.Thursday).OrderBy(a => a.FromHour.TimeOfDay),
                 //Friday = availabilitiesDTO.Where(a => a.DayOfWeek == DayOfWeek.Friday).OrderBy(a => a.FromHour.TimeOfDay),
                 //Saturday = availabilitiesDTO.Where(a => a.DayOfWeek == DayOfWeek.Saturday).OrderBy(a => a.FromHour.TimeOfDay),
                 Sorted = new Dictionary<TimeSpan, IEnumerable<AvailabilityDTO>> ()
            };

            foreach (var h in vm.Hours)
            {
                vm.Sorted.Add(h, availabilitiesDTO
                .Where(a => a.StartTime.TimeOfDay == h)
                .OrderBy(a => a.StartTime.TimeOfDay));
            }
            //vm.Sorted.Add(DayOfWeek.Sunday, availabilitiesDTO
            //    .Where(a => a.DayOfWeek == DayOfWeek.Sunday)
            //    .OrderBy(a => a.FromHour.TimeOfDay));
            // vm.Sorted.Add(DayOfWeek.Monday, availabilitiesDTO
            //    .Where(a => a.DayOfWeek == DayOfWeek.Monday)
            //    .OrderBy(a => a.FromHour.TimeOfDay));
            //vm.Sorted.Add(DayOfWeek.Tuesday, availabilitiesDTO
            //    .Where(a => a.DayOfWeek == DayOfWeek.Tuesday)
            //    .OrderBy(a => a.FromHour.TimeOfDay));
            //vm.Sorted.Add(DayOfWeek.Wednesday, availabilitiesDTO
            //    .Where(a => a.DayOfWeek == DayOfWeek.Wednesday)
            //    .OrderBy(a => a.FromHour.TimeOfDay));
            //vm.Sorted.Add(DayOfWeek.Thursday, availabilitiesDTO
            //    .Where(a => a.DayOfWeek == DayOfWeek.Thursday)
            //    .OrderBy(a => a.FromHour.TimeOfDay));
            //vm.Sorted.Add(DayOfWeek.Friday, availabilitiesDTO
            //    .Where(a => a.DayOfWeek == DayOfWeek.Friday)
            //    .OrderBy(a => a.FromHour.TimeOfDay));
            //vm.Sorted.Add(DayOfWeek.Saturday, availabilitiesDTO
            //    .Where(a => a.DayOfWeek == DayOfWeek.Saturday)
            //    .OrderBy(a => a.FromHour.TimeOfDay));

            //var grouped = availabilitiesDTO
            //    .
            //.OrderBy(a => a.Key)
            //.Select(k => new Keys()
            //{ 
            //DayOfWeek = k.Key.DayOfWeek,
            //FromHour = k.Key.FromHour,
            //AvailabilityDTOs = k.ToList()
            //}).ToList();

            return View(vm);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLAvailabilitiesBuilder.GetById(_apiPath, id.Value);
            AvailabilityDTO availabilityDTO = null;

            try
            {
                availabilityDTO = await _availabilitiesDownLoader.GetByIdAsync(url, HttpContext.Session);
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

            if (availabilityDTO == null)
            {
                return NotFound();
            }

            GetViewData();

            return View(availabilityDTO);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLAvailabilitiesBuilder.GetById(_apiPath, id.Value);

            AvailabilityDTO availabilityDTO = null;

            try
            {
                availabilityDTO = await _availabilitiesDownLoader.GetByIdAsync(url, HttpContext.Session);
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

            return View(availabilityDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AvailabilityDTO availabilityDTO)
        {
            if (id != availabilityDTO.ID)
            {
                return NotFound();
            }

            var url = _URLAvailabilitiesBuilder.GetById(_apiPath, id);
            var status = await _availabilitiesDownLoader.CreatePutAsync(url, availabilityDTO,
                                                            HttpContext.Session);

            if (status != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            GetViewData();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            GetViewData();
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

        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var url = _URLAvailabilitiesBuilder.GetById(_apiPath, id);
            var status = await _availabilitiesDownLoader.DeleteAsync(url, HttpContext.Session);

            if (status != HttpStatusCode.OK)
            {
                return Redirect("Home/Error");
            }

            GetViewData();

            return RedirectToAction(nameof(Index));
        }

        private void GetViewData()
        {
            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                           ViewData);
        }

    }
}