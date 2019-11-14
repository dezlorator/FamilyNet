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
using Microsoft.AspNetCore.Http;
using DataTransferObjects.Enums;

namespace FamilyNet.Controllers
{
    public class ChildrenActivitiesController : Controller
    {
        #region private fields

        private readonly IIdentityInformationExtractor _identityInformationExtactor;
        private readonly ServerSimpleDataDownloader<ChildActivityDTO> _childrenActivitiesDownloader;
        private readonly IURLChildrenActivitesBuilder _URLChildrenActivitiesBuilder;
        private readonly string _apiChildrenActivitiesPath = "api/v1/childrenActivities";
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

            ViewBag.ChildID = searchModel.ChildID;

            GetViewData();

            return View(childrenActivities);
        }

        public IActionResult Create(int childId)
        {
            var role = HttpContext.Session.GetString("roles");
            if (GetUserRoleByString(role) == UserRole.Undefined)
            {
                return Redirect("/Home/Error");
            }

            if (childId <= 0)
            {
                return NotFound();
            }

            GetViewData();

            return View(new ChildActivityDTO { ChildID = childId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChildActivityDTO childActivityDTO)
        {
            var role = HttpContext.Session.GetString("roles");
            if (GetUserRoleByString(role) == UserRole.Undefined)
            {
                return Redirect("/Home/Error");
            }

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
            var role = HttpContext.Session.GetString("roles");
            if (GetUserRoleByString(role) == UserRole.Undefined)
            {
                return Redirect("/Home/Error");
            }

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
        public async Task<IActionResult> Edit(int id, ChildActivityDTO childActivityDTO)
        {
            var role = HttpContext.Session.GetString("roles");
            if (GetUserRoleByString(role) == UserRole.Undefined)
            {
                return Redirect("/Home/Error");
            }

            if (id != childActivityDTO.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(childActivityDTO);
            }

            var url = _URLChildrenActivitiesBuilder.GetById(_apiChildrenActivitiesPath, id);

            var message = await _childrenActivitiesDownloader.CreatePutAsync(url, childActivityDTO,
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

        public IActionResult AddAward(int? childActivityId)
        {
            var role = HttpContext.Session.GetString("roles");
            if (GetUserRoleByString(role) == UserRole.Undefined)
            {
                return Redirect("/Home/Error");
            }

            if (childActivityId == null)
            {
                return NotFound();
            }

            GetViewData();

            return View(new AwardViewModel { ChildActivityID = (int)childActivityId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAward(AwardViewModel award)
        {
            var role = HttpContext.Session.GetString("roles");
            if (GetUserRoleByString(role) == UserRole.Undefined)
            {
                return Redirect("/Home/Error");
            }

            var url = _URLChildrenActivitiesBuilder.GetById(_apiChildrenActivitiesPath, award.ChildActivityID);
            ChildActivityDTO childActivityDTO = null;

            try
            {
                childActivityDTO = await _childrenActivitiesDownloader.GetByIdAsync(url, HttpContext.Session);
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

            if (childActivityDTO == null)
            {
                return NotFound();
            }

            childActivityDTO.Awards.Add(new AwardDTO
            {
                Name = award.Name,
                Description = award.Description,
                Date = award.Date
            });

            var message = await _childrenActivitiesDownloader.CreatePutAsync(url, childActivityDTO,
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

        public async Task<IActionResult> Delete(int id)
        {
            var role = HttpContext.Session.GetString("roles");
            if (GetUserRoleByString(role) == UserRole.Undefined)
            {
                return Redirect("/Home/Error");
            }

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

            if (message.StatusCode != HttpStatusCode.OK)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return Redirect("/Orphans/Index");
        }

        public async Task<IActionResult> DeleteAward(int activityId, int awardId)
        {
            var role = HttpContext.Session.GetString("roles");
            if (GetUserRoleByString(role) == UserRole.Undefined)
            {
                return Redirect("/Home/Error");
            }

            if (activityId <= 0 || awardId <= 0)
            {
                return NotFound();
            }

            var url = _URLChildrenActivitiesBuilder.GetById(_apiChildrenActivitiesPath, activityId);
            ChildActivityDTO childActivityDTO = null;

            try
            {
                childActivityDTO = await _childrenActivitiesDownloader.GetByIdAsync(url, HttpContext.Session);
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

            if (childActivityDTO == null)
            {
                return NotFound();
            }

            var award = childActivityDTO.Awards.FirstOrDefault(a => a.ID == awardId);
            childActivityDTO.Awards.Remove(award);

            var message = await _childrenActivitiesDownloader.CreatePutAsync(url, childActivityDTO,
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

        private void GetViewData()
        {
            ViewData["ChildrenActivities"] = _localizer["ChildrenActivities"];

            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                                 ViewData);
        }

        private UserRole GetUserRoleByString(string str)
        {
            switch (str)
            {
                case "Admin":
                    return UserRole.Admin;
                case "Representative":
                    return UserRole.Representative;
            }
            return UserRole.Undefined;
        }
    }
}