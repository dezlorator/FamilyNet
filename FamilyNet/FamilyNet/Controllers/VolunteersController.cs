﻿using System;
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
    public class VolunteersController : BaseController
    {
        #region private fields

        private readonly IStringLocalizer<VolunteersController> _localizer;
        private readonly ServerDataDownLoader<VolunteerDTO> _downLoader;
        private readonly IURLVolunteersBuilder _URLVolunteersBuilder;
        private readonly string _apiPath = "api/v1/volunteers";
        private readonly IFileStreamCreater _streamCreater;

        #endregion

        #region ctor

        public VolunteersController(IUnitOfWorkAsync unitOfWork,
                                 IStringLocalizer<VolunteersController> localizer,
                                 ServerDataDownLoader<VolunteerDTO> downLoader,
                                 IURLVolunteersBuilder URLChildrenBuilder,
                                 IFileStreamCreater streamCreater)
            : base(unitOfWork)
        {
            _localizer = localizer;
            _downLoader = downLoader;
            _URLVolunteersBuilder = URLChildrenBuilder;
            _streamCreater = streamCreater;
        }

        #endregion

        [AllowAnonymous]
        public async Task<IActionResult> Index(int id, PersonSearchModel searchModel)
        {
            var url = _URLVolunteersBuilder.GetAllWithFilter(_apiPath,
                                                           searchModel,
                                                           id);
            IEnumerable<VolunteerDTO> volunteers = null;

            try
            {
                volunteers = await _downLoader.GetAllAsync(url, HttpContext.Session);
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

            var selectedVolunteers = volunteers.Select(volunteer => new Volunteer()
            {
                Birthday = volunteer.Birthday,
                FullName = new FullName()
                {
                    Name = volunteer.Name,
                    Patronymic = volunteer.Patronymic,
                    Surname = volunteer.Surname
                },
                ID = volunteer.ID,
                Avatar = volunteer.PhotoPath,
                AddressID = volunteer.AddressID,
                EmailID = volunteer.EmailID,
                Rating = volunteer.Rating
            });

            GetViewData();

            return View(selectedVolunteers);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLVolunteersBuilder.GetById(_apiPath, id.Value);
            VolunteerDTO volunteerDTO = null;

            try
            {
                volunteerDTO = await _downLoader.GetByIdAsync(url, HttpContext.Session);
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

            if (volunteerDTO == null)
            {
                return NotFound();
            }

            var volunteer = new Volunteer()
            {
                Birthday = volunteerDTO.Birthday,
                FullName = new FullName()
                {
                    Name = volunteerDTO.Name,
                    Patronymic = volunteerDTO.Patronymic,
                    Surname = volunteerDTO.Surname
                },
                ID = volunteerDTO.ID,
                Avatar = volunteerDTO.PhotoPath,
                AddressID = volunteerDTO.AddressID,
                EmailID = volunteerDTO.EmailID,
                Rating = volunteerDTO.Rating,
            };

            GetViewData();

            return View(volunteer);
        }

        [Authorize(Roles = "Admin, Volunteer")]
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
        [Authorize(Roles = "Admin, Volunteer")]
        public async Task<IActionResult> Create([Bind("Name,Surname,Patronymic,Birthday,AddressID,Avatar")]
                                                VolunteerDTO volunteerDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(volunteerDTO);
            }

            Stream stream = null;

            if (volunteerDTO.Avatar != null)
            {
                stream = _streamCreater.CopyFileToStream(volunteerDTO.Avatar);
            }

            var url = _URLVolunteersBuilder.CreatePost(_apiPath);
            var status = await _downLoader.CreatePostAsync(url, volunteerDTO,
                                                             stream, volunteerDTO.Avatar.FileName,
                                                             HttpContext.Session);

            if (status != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            return Redirect("/Volunteers/Index");
        }

        [Authorize(Roles = "Admin, Volunteer")]
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

            var url = _URLVolunteersBuilder.GetById(_apiPath, id.Value);
            VolunteerDTO volunteerDTO = null;

            try
            {
                volunteerDTO = await _downLoader.GetByIdAsync(url, HttpContext.Session);
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

            return View(volunteerDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Volunteer")]
        public async Task<IActionResult> Edit(int id, VolunteerDTO volunteerDTO)
        {
            if (id != volunteerDTO.ID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(volunteerDTO);
            }

            Stream stream = null;

            if (volunteerDTO.Avatar != null)
            {
                stream = _streamCreater.CopyFileToStream(volunteerDTO.Avatar);
            }

            var url = _URLVolunteersBuilder.GetById(_apiPath, id);
            var status = await _downLoader.CreatePutAsync(url, volunteerDTO,
                                                            stream, volunteerDTO.Avatar?.FileName,
                                                            HttpContext.Session);

            if (status != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            return Redirect("/Volunteers/Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLVolunteersBuilder.GetById(_apiPath, id.Value);
            VolunteerDTO volunteerDTO = null;

            try
            {
                volunteerDTO = await _downLoader.GetByIdAsync(url, HttpContext.Session);
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

            if (volunteerDTO == null)
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

            var url = _URLVolunteersBuilder.GetById(_apiPath, id);
            var status = await _downLoader.DeleteAsync(url, HttpContext.Session);

            if (status != HttpStatusCode.OK)
            {
                return Redirect("/Home/Error");
            }

            GetViewData();

            return Redirect("/Volunteers/Index");
        }

        [AllowAnonymous]
        public async Task<IActionResult> VolunteersTable(int id, PersonSearchModel searchModel)
        {
            var url = _URLVolunteersBuilder.GetAllWithFilter(_apiPath,
                                                           searchModel,
                                                           id);
            IEnumerable<VolunteerDTO> volunteers = null;

            try
            {
                volunteers = await _downLoader.GetAllAsync(url, HttpContext.Session);
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

            var selectedVolunteers = volunteers.Select(volunteer => new Volunteer()
            {
                Birthday = volunteer.Birthday,
                FullName = new FullName()
                {
                    Name = volunteer.Name,
                    Patronymic = volunteer.Patronymic,
                    Surname = volunteer.Surname
                },
                ID = volunteer.ID,
                Avatar = volunteer.PhotoPath,
                AddressID = volunteer.AddressID,
                EmailID = volunteer.EmailID,
                Rating = volunteer.Rating
            });

            GetViewData();

            return View(selectedVolunteers);
        }

        private void GetViewData()
        {
            ViewData["VolunteersList"] = _localizer["VolunteersList"];
        }
    }
}
