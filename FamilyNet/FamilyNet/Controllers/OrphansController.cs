using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
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
    public class OrphansController : Controller
    {
        #region private fields

        private readonly ServerDataDownloader<ChildrenHouseDTO> _childrenHouseDownloader;
        private readonly IStringLocalizer<OrphansController> _localizer;
        private readonly ServerDataDownloader<ChildDTO> _childrenDownloader;
        private readonly IURLChildrenBuilder _URLChildrenBuilder;
        private readonly IURLChildrenHouseBuilder _URLChildrenHouseBuilder;
        private readonly string _apiChildrenPath = "api/v1/children";
        private readonly string _apiChildrenHousesPath = "api/v1/childrenhouse";
        private readonly IFileStreamCreater _streamCreater;

        #endregion

        #region ctor

        public OrphansController(IStringLocalizer<OrphansController> localizer,
                                 ServerDataDownloader<ChildDTO> childrenDownloader,
                                 ServerDataDownloader<ChildrenHouseDTO> childrenHouseDownloader,
                                 IURLChildrenBuilder URLChildrenBuilder,
                                 IURLChildrenHouseBuilder URLChildrenHouseBuilder,
                                 IFileStreamCreater streamCreater)
        {
            _localizer = localizer;
            _childrenDownloader = childrenDownloader;
            _childrenHouseDownloader = childrenHouseDownloader;
            _URLChildrenBuilder = URLChildrenBuilder;
            _URLChildrenHouseBuilder = URLChildrenHouseBuilder;
            _streamCreater = streamCreater;
        }

        #endregion

        public async Task<IActionResult> Index(int id, PersonSearchModel searchModel)
        {
            var url = _URLChildrenBuilder.GetAllWithFilter(_apiChildrenPath,
                                                           searchModel,
                                                           id);
            IEnumerable<ChildDTO> children = null;

            try
            {
                children = await _childrenDownloader.GetAllAsync(url, HttpContext.Session);
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

            return View(children);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLChildrenBuilder.GetById(_apiChildrenPath, id.Value);
            ChildDTO childDTO = null;

            try
            {
                childDTO = await _childrenDownloader.GetByIdAsync(url, HttpContext.Session);
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

            if (childDTO == null)
            {
                return NotFound();
            }

            GetViewData();

            return View(childDTO);
        }

        public async Task<IActionResult> Create()
        {
            var urlChildrenHouse = _URLChildrenHouseBuilder.GetAllWithFilter(_apiChildrenHousesPath,
                                                                            new OrphanageSearchModel(),
                                                                            SortStateOrphanages.NameAsc);
            try
            {
                var childrenHouses = await _childrenHouseDownloader.GetAllAsync(urlChildrenHouse, HttpContext.Session);
                ViewBag.ListOfOrphanages = childrenHouses;
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

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Surname,Patronymic,Birthday,ChildrenHouseID,Avatar")]
                                                ChildDTO childDTO)
        {
            HttpStatusCode status = HttpStatusCode.BadRequest;

            if (!ModelState.IsValid)
            {
                return View(childDTO);
            }

            Stream stream = null;

            if (childDTO.Avatar != null)
            {
                stream = _streamCreater.CopyFileToStream(childDTO.Avatar);
            }

            try
            {
                var url = _URLChildrenBuilder.CreatePost(_apiChildrenPath);
                status = await _childrenDownloader.CreatePostAsync(url, childDTO,
                                                                 stream,
                                                                 childDTO.Avatar?.FileName,
                                                                 HttpContext.Session);
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

            if (status != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            return Redirect("/Orphans/Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urlChildren = _URLChildrenBuilder.GetById(_apiChildrenPath, id.Value);
            var urlChildrenHouse = _URLChildrenHouseBuilder.GetAllWithFilter(_apiChildrenHousesPath,
                                                                              new OrphanageSearchModel(),
                                                                              SortStateOrphanages.NameAsc);

            ChildDTO childDTO = null;

            try
            {
                childDTO = await _childrenDownloader.GetByIdAsync(urlChildren, HttpContext.Session);
                var childrenHouses = await _childrenHouseDownloader.GetAllAsync(urlChildrenHouse,
                                                                                HttpContext.Session);
                ViewBag.ListOfOrphanages = childrenHouses;
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

            return View(childDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChildDTO childDTO)
        {
            if (id != childDTO.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(childDTO);
            }

            Stream stream = null;

            if (childDTO.Avatar != null)
            {
                stream = _streamCreater.CopyFileToStream(childDTO.Avatar);
            }

            var urlChildren = _URLChildrenBuilder.GetById(_apiChildrenPath, id);
            HttpStatusCode status = HttpStatusCode.BadRequest;

            try
            {
                status = await _childrenDownloader.CreatePutAsync(urlChildren, childDTO,
                                                  stream, childDTO.Avatar?.FileName,
                                                  HttpContext.Session);
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

            if (status == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
                //TODO: log
            }

            if (status != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
            }

            return Redirect("/Orphans/Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLChildrenBuilder.GetById(_apiChildrenPath, id.Value);
            ChildDTO childDTO = null;

            try
            {
                childDTO = await _childrenDownloader.GetByIdAsync(url, HttpContext.Session);
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

            if (childDTO == null)
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

            var url = _URLChildrenBuilder.GetById(_apiChildrenPath, id);
            HttpStatusCode status = HttpStatusCode.BadRequest;

            try
            {
                status = await _childrenDownloader.DeleteAsync(url, HttpContext.Session);
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

            if (status != HttpStatusCode.OK)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return Redirect("/Orphans/Index");
        }

        public async Task<IActionResult> OrphansTable(int id, PersonSearchModel searchModel)
        {
            var url = _URLChildrenBuilder.GetAllWithFilter(_apiChildrenPath,
                                                           searchModel,
                                                           id);
            IEnumerable<ChildDTO> children = null;

            try
            {
                children = await _childrenDownloader.GetAllAsync(url, HttpContext.Session);
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

            var orphans = children.Select(child => new Orphan()
            {
                Birthday = child.Birthday,
                FullName = new FullName()
                {
                    Name = child.Name,
                    Patronymic = child.Patronymic,
                    Surname = child.Surname
                },
                ID = child.ID,
                Avatar = child.PhotoPath,
                OrphanageID = child.ChildrenHouseID,
                EmailID = child.EmailID,
                Rating = child.Rating

            });

            GetViewData();

            return View(orphans);
        }

        private void GetViewData()
        {
            ViewData["OrphansList"] = _localizer["OrphansList"];
        }
    }
}