using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Http;
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

        private readonly IStringLocalizer<OrphansController> _localizer;
        private readonly ServerDataDownLoader<ChildDTO> _downLoader;
        private readonly IURLChildrenBuilder _URLChildrenBuilder;
        private readonly string _apiPath = "api/v1/children";
        private readonly IFileStreamCreater _streamCreater;

        #endregion

        #region ctor

        public OrphansController(IUnitOfWorkAsync unitOfWork,
                                 IStringLocalizer<OrphansController> localizer,
                                 ServerDataDownLoader<ChildDTO> downLoader,
                                 IURLChildrenBuilder URLChildrenBuilder,
                                 IFileStreamCreater streamCreater)
            : base(unitOfWork)
        {
            _localizer = localizer;
            _downLoader = downLoader;
            _URLChildrenBuilder = URLChildrenBuilder;
            _streamCreater = streamCreater;
        }

        #endregion

        [AllowAnonymous]
        public async Task<IActionResult> Index(int id, PersonSearchModel searchModel)
        {
            var url = _URLChildrenBuilder.GetAllWithFilter(_apiPath,
                                                           searchModel,
                                                           id);
            IEnumerable<ChildDTO> children = null;

            try
            {
                children = await _downLoader.GetAllAsync(url, Request);
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
        
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLChildrenBuilder.GetById(_apiPath, id.Value);
            ChildDTO childDTO = null;

            try
            {
                childDTO = await _downLoader.GetByIdAsync(url, Request);
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

            var orphan = new Orphan()
            {
                Birthday = childDTO.Birthday,
                FullName = new FullName()
                {
                    Name = childDTO.Name,
                    Patronymic = childDTO.Patronymic,
                    Surname = childDTO.Surname
                },
                ID = childDTO.ID,
                Avatar = childDTO.PhotoPath,
                OrphanageID = childDTO.ChildrenHouseID,
                EmailID = childDTO.EmailID,
                Rating = childDTO.Rating,
            };

            GetViewData();

            return View(orphan);
        }

        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Create()
        {
            await Check();

            var orphanagesList = new List<Orphanage>();
            orphanagesList = _unitOfWorkAsync.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;
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

            var url = _URLChildrenBuilder.CreatePost(_apiPath);
            var status = await _downLoader.СreatetePostAsync(url, childDTO,
                                                             stream,
                                                             childDTO.Avatar.FileName,
                                                             Request);

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

            var url = _URLChildrenBuilder.GetById(_apiPath, id.Value);
            ChildDTO childDTO = null;

            try
            {
                childDTO = await _downLoader.GetByIdAsync(url, Request);
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

            //TODO: ChildrenHouseAPI
            var orphanagesList = _unitOfWorkAsync.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;

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

            var url = _URLChildrenBuilder.GetById(_apiPath, id);
            var status = await _downLoader.СreatetePutAsync(url, childDTO,
                                                            stream, childDTO.Avatar?.FileName,
                                                            Request);

            if (status != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
                //TODO: log
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

            var url = _URLChildrenBuilder.GetById(_apiPath, id.Value);
            ChildDTO childDTO = null;

            try
            {
                childDTO = await _downLoader.GetByIdAsync(url, Request);
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

            var url = _URLChildrenBuilder.GetById(_apiPath, id);
            var status = await _downLoader.DeleteAsync(url, Request);

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
            var url = _URLChildrenBuilder.GetAllWithFilter(_apiPath,
                                                           searchModel,
                                                           id);
            IEnumerable<ChildDTO> children = null;

            try
            {
                children = await _downLoader.GetAllAsync(url, Request);
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