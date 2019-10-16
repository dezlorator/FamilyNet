using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Models.ViewModels;
using FamilyNet.Infrastructure;
using DataTransferObjects;
using FamilyNet.Downloader;
using Microsoft.Extensions.Localization;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using FamilyNet.StreamCreater;
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
                children = await _downLoader.GetAllAsync(url);
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
                childDTO = await _downLoader.GetByIdAsync(url);
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
        public IActionResult Create()
        {
            Check();

            List<Orphanage> orphanagesList = new List<Orphanage>();
            orphanagesList = _unitOfWorkAsync.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;
            GetViewData();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Create([Bind("FullName,Address,Birthday,Orphanage,Avatar")]
                                                Orphan orphan, int id, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return View(orphan);
            }

            var childDTO = new ChildDTO()
            {
                Birthday = orphan.Birthday,
                Surname = orphan.FullName.Surname,
                Name = orphan.FullName.Name,
                Patronymic = orphan.FullName.Patronymic,
                Rating = orphan.Rating,
                EmailID = orphan.EmailID,
                ChildrenHouseID = id,
            };

            Stream stream = null;

            if (file != null)
            {
                stream = _streamCreater.CopyFileToStream(file);
            }

            var url = _URLChildrenBuilder.CreatePost(_apiPath);
            var status = await _downLoader.СreatetePostAsync(url, childDTO,
                                                             stream, file.FileName);

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

            List<Orphanage> orphanagesList = _unitOfWorkAsync.Orphanages.GetAll().ToList();
            ViewBag.ListOfOrphanages = orphanagesList;

            var orphan = await _unitOfWorkAsync.Orphans.GetById((int)id);

            if (orphan == null)
            {
                return NotFound();
            }
            GetViewData();

            return View(orphan);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Orphan")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FullName,Birthday,Orphanage,Avatar")] Orphan orphan, int idOrphanage, IFormFile file)
        {
            if (id != orphan.ID)
            {
                return NotFound();
            }

            var check = CheckById((int)id).Result;
            var checkResult = check != null;
            if (checkResult)
            {
                return check;
            }

            await ImageHelper.SetAvatar(orphan, file, "wwwroot\\children");


            if (ModelState.IsValid)
            {
                try
                {
                    var orphanage = await _unitOfWorkAsync.Orphanages.GetById(idOrphanage);
                    orphan.Orphanage = orphanage;
                    var orphanToEdit = await _unitOfWorkAsync.Orphans.GetById(orphan.ID);
                    orphanToEdit.CopyState(orphan);
                    _unitOfWorkAsync.Orphans.Update(orphanToEdit);
                    _unitOfWorkAsync.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_unitOfWorkAsync.Orphans.Any(orphan.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            GetViewData();

            return View(orphan);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orphan = await _unitOfWorkAsync.Orphans.GetById((int)id);
            if (orphan == null)
            {
                return NotFound();
            }
            GetViewData();

            return View(orphan);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orphan = await _unitOfWorkAsync.Orphans.GetById((int)id);
            if (orphan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            await _unitOfWorkAsync.Orphans.Delete((int)id);
            _unitOfWorkAsync.SaveChangesAsync();
            GetViewData();

            return RedirectToAction(nameof(Index));
        }


        [AllowAnonymous]
        public IActionResult OrphansTable(int id, PersonSearchModel searchModel)
        {
            IEnumerable<Orphan> orphans = _unitOfWorkAsync.Orphans.GetAll();

            orphans = OrphanFilter.GetFiltered(orphans, searchModel);

            if (id == 0)
                return View(orphans);

            if (id > 0)
                orphans = orphans.Where(x => x.Orphanage.ID.Equals(id)).ToList();

            GetViewData();

            return View(orphans);
        }


        private void GetViewData()
        {
            ViewData["OrphansList"] = _localizer["OrphansList"];
        }


        private bool OrphanExists(int id)
        {
            return _unitOfWorkAsync.Orphans.GetById(id) != null;
        }
    }
}
