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
using FamilyNet.IdentityHelpers;

namespace FamilyNet.Controllers
{
    public class ChildrenActivitiesController : Controller
    {
        #region private fields

        private readonly IIdentityInformationExtractor _identityInformationExtactor;
        private readonly ServerSimpleDataDownloader<ChildActivityDTO> _childrenActivitiesDownloader;
        private readonly IURLChildrenActivitesBuilder _URLChildrenActivitiesBuilder;
        private readonly string _apiChildrenActivitiesPath = "api/v2/childrenActivities";
        private readonly IStringLocalizer<ChildrenActivitiesController> _localizer;

        #endregion

        #region ctor

        public ChildrenActivitiesController(ServerSimpleDataDownloader<ChildActivityDTO> childrenActivitiesDownloader,
                                 IURLChildrenActivitesBuilder URLChildrenActivitiesBuilder,
                                 IIdentityInformationExtractor identityInformationExtactor,
                                 IStringLocalizer<ChildrenActivitiesController> localizer)
        {
            _childrenActivitiesDownloader = childrenActivitiesDownloader;
            _URLChildrenActivitiesBuilder = URLChildrenActivitiesBuilder;
            _identityInformationExtactor = identityInformationExtactor;
            _localizer = localizer;
        }

        #endregion

        public async Task<IActionResult> Index(ChildActivitySearchModel searchModel)
        {
            var url = _URLChildrenActivitiesBuilder.GetAllWithFilter(_apiChildrenActivitiesPath,
                                                                     searchModel);
            IEnumerable<ChildActivityDTO> childrenActivities = null;

            try
            {
                childrenActivities = await _childrenActivitiesDownloader.GetAllAsync(url, HttpContext.Session);
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

            return View(childrenActivities);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Redirect("/Orphans/Index");
            }

            var url = _URLChildrenActivitiesBuilder.GetById(_apiChildrenActivitiesPath, id.Value);
            ChildActivityDTO ChildActivityDTO = null;

            try
            {
                ChildActivityDTO = await _childrenActivitiesDownloader.GetByIdAsync(url, HttpContext.Session);
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

            if (ChildActivityDTO == null)
            {
                return NotFound();
            }

            GetViewData();

            return View(ChildActivityDTO);
        }

        public IActionResult Create()
        {
            GetViewData();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChildActivityDTO childActivityDTO)
        {
            var url = _URLChildrenActivitiesBuilder.CreatePost(_apiChildrenActivitiesPath);
            var message = await _childrenActivitiesDownloader.CreatePostAsync(url, childActivityDTO,
                                                                 HttpContext.Session);

            if (message.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (message.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return Redirect("/Orphans/Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urlChildActivity = _URLChildrenActivitiesBuilder.GetById(_apiChildrenActivitiesPath, id.Value);

            ChildActivityDTO childActivityDTO = null;

            try
            {
                childActivityDTO = await _childrenActivitiesDownloader.GetByIdAsync(urlChildActivity, HttpContext.Session);
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

            return View(childActivityDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChildActivityDTO ChildActivityDTO)
        {
            if (id != ChildActivityDTO.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(ChildActivityDTO);
            }

            var urlChildren = _URLChildrenActivitiesBuilder.GetById(_apiChildrenActivitiesPath, id);

            var message = await _childrenActivitiesDownloader.CreatePutAsync(urlChildren, ChildActivityDTO,
                                              HttpContext.Session);

            if (message.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (message.StatusCode != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return Redirect("/Orphans/Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLChildrenActivitiesBuilder.GetById(_apiChildrenActivitiesPath, id.Value);
            ChildActivityDTO ChildActivityDTO = null;

            try
            {
                ChildActivityDTO = await _childrenActivitiesDownloader.GetByIdAsync(url, HttpContext.Session);
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

            if (ChildActivityDTO == null)
            {
                return NotFound();
            }

            GetViewData();

            return View(id.Value);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var url = _URLChildrenActivitiesBuilder.GetById(_apiChildrenActivitiesPath, id);

            var message = await _childrenActivitiesDownloader.DeleteAsync(url, HttpContext.Session);

            if (message.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (message.StatusCode != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return Redirect("/Orphans/Index");
        }

        private void GetViewData()
        {
            ViewData["ChildrenActivities"] = _localizer["ChildrenActivities"];

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                                 ViewData);
        }
    }
}