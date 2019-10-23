using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class OrphansController : BaseController
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

        public OrphansController(IUnitOfWorkAsync unitOfWork,
                                 IStringLocalizer<OrphansController> localizer,
                                 ServerDataDownloader<ChildDTO> childrenDownloader,
                                 ServerDataDownloader<ChildrenHouseDTO> childrenHouseDownloader,
                                 IURLChildrenBuilder URLChildrenBuilder,
                                 IURLChildrenHouseBuilder URLChildrenHouseBuilder,
                                 IFileStreamCreater streamCreater)
            : base(unitOfWork)
        {
            _localizer = localizer;
            _childrenDownloader = childrenDownloader;
            _childrenHouseDownloader = childrenHouseDownloader;
            _URLChildrenBuilder = URLChildrenBuilder;
            _URLChildrenHouseBuilder = URLChildrenHouseBuilder;
            _streamCreater = streamCreater;
        }

        #endregion

        [AllowAnonymous]
        public async Task<IActionResult> Index(int id, PersonSearchModel searchModel)
        {
            var url = _URLChildrenBuilder.GetAllWithFilter(_apiChildrenPath,
                                                           searchModel,
                                                           id);
            IEnumerable<ChildDTO> children = null;

            try
            {
                children = await _childrenDownloader.GetAllAsync(url);
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

        [AllowAnonymous]
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
                childDTO = await _childrenDownloader.GetByIdAsync(url);
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

        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Create()
        {
            await Check();

            var urlChildrenHouse = _URLChildrenHouseBuilder.GetAllWithFilter(_apiChildrenHousesPath,
                                                                            new OrphanageSearchModel(),
                                                                            SortStateOrphanages.NameAsc);
            try
            {
                var childrenHouses = await _childrenHouseDownloader.GetAllAsync(urlChildrenHouse);
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
        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Create([Bind("Name,Surname,Patronymic,Birthday,ChildrenHouseID,Avatar")]
                                                ChildDTO childDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(childDTO);
            }

            Stream stream = null;

            if (childDTO.Avatar != null)
            {
                stream = _streamCreater.CopyFileToStream(childDTO.Avatar);
            }

            var url = _URLChildrenBuilder.CreatePost(_apiChildrenPath);           

            HttpStatusCode status = HttpStatusCode.BadRequest;

            try
            {
                status = await _childrenDownloader.СreatePostAsync(url, childDTO,
                                                                 stream, childDTO.Avatar?.FileName);
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

        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var check = CheckById((int)id).Result;
            var checkResult = check != null;
            if (checkResult)
            {
                return check;
            }

            var urlChildren = _URLChildrenBuilder.GetById(_apiChildrenPath, id.Value);
            var urlChildrenHouse = _URLChildrenHouseBuilder.GetAllWithFilter(_apiChildrenHousesPath,
                                                                              new OrphanageSearchModel(), 
                                                                              SortStateOrphanages.NameAsc);

            ChildDTO childDTO = null;

            try
            {
                childDTO = await _childrenDownloader.GetByIdAsync(urlChildren);
                var childrenHouses = await _childrenHouseDownloader.GetAllAsync(urlChildrenHouse);
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
        [Authorize(Roles = "Admin, Orphan")]
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

            var url = _URLChildrenBuilder.GetById(_apiChildrenPath, id);
            HttpStatusCode status = HttpStatusCode.BadRequest;

            try
            {
                status = await _childrenDownloader.СreatePutAsync(url, childDTO,
                                                  stream, childDTO.Avatar?.FileName);
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

            if (status != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
            }

            return Redirect("/Orphans/Index");
        }

        [Authorize(Roles = "Admin")]
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
                childDTO = await _childrenDownloader.GetByIdAsync(url);
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
        [Authorize(Roles = "Admin")]
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
                status = await _childrenDownloader.DeleteAsync(url);
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

        [AllowAnonymous]
        public async Task<IActionResult> OrphansTable(int id, PersonSearchModel searchModel)
        {
            var url = _URLChildrenBuilder.GetAllWithFilter(_apiChildrenPath,
                                                           searchModel,
                                                           id);
            IEnumerable<ChildDTO> children = null;

            try
            {
                children = await _childrenDownloader.GetAllAsync(url);
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