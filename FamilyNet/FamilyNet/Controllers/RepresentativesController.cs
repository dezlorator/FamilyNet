﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FamilyNet.Models;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Models.ViewModels;
using FamilyNet.Infrastructure;
using FamilyNet.Downloader;
using DataTransferObjects;
using System.Net.Http;
using Newtonsoft.Json;
using FamilyNet.StreamCreater;
using System.Net;
using Microsoft.Extensions.Options;
using FamilyNet.Configuration;

namespace FamilyNet.Controllers
{
    [Authorize]
    public class RepresentativesController : BaseController
    {
        #region private fields

        private readonly ServerDataDownLoader<RepresentativeDTO> _downLoader;
        private readonly IURLRepresentativeBuilder _URLRepresentativeBuilder;
        private readonly string _apiPath = "api/v1/representatives";
        private readonly IFileStreamCreater _streamCreater;



        #endregion
        #region Ctor

        public RepresentativesController(IUnitOfWorkAsync unitOfWork,
            IURLRepresentativeBuilder urlRepresentativeBuilder,
            ServerDataDownLoader<RepresentativeDTO> downloader,
            IFileStreamCreater streamCreater) : base(unitOfWork)
        {
            _URLRepresentativeBuilder = urlRepresentativeBuilder;
            _downLoader = downloader;
            _streamCreater = streamCreater;
        }

        #endregion

        #region Methods

        // GET: Representatives
        [AllowAnonymous]
        public async Task<IActionResult> Index(int id,
            PersonSearchModel searchModel)
        {
            var url = _URLRepresentativeBuilder
                .GetAllWithFilter(_apiPath, searchModel, id);

            IEnumerable<RepresentativeDTO> representativesDTO = null;

            try
            {
                representativesDTO = await _downLoader.GetAllAsync(url);
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

            var representatives = representativesDTO.Select(r => new Representative()
            {
                Birthday = r.Birthday,
                FullName = new FullName()
                {
                    Name = r.Name,
                    Patronymic = r.Patronymic,
                    Surname = r.Surname
                },
                ID = r.ID,
                Avatar = r.PhotoPath,
                OrphanageID = r.ChildrenHouseID,
                EmailID = r.EmailID,
                Rating = r.Rating
            });

            return View(representatives);
        }

        // GET: Representatives/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLRepresentativeBuilder.GetById(_apiPath, id.Value);
            RepresentativeDTO representativeDTO = null;

            try
            {
                representativeDTO = await _downLoader.GetByIdAsync(url);
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

            if (representativeDTO == null)
            {
                return NotFound();
            }

            var representative = new Representative()
            {
                Birthday = representativeDTO.Birthday,
                FullName = new FullName()
                {
                    Name = representativeDTO.Name,
                    Patronymic = representativeDTO.Patronymic,
                    Surname = representativeDTO.Surname
                },
                ID = representativeDTO.ID,
                Avatar = representativeDTO.PhotoPath,
                OrphanageID = representativeDTO.ChildrenHouseID,
                EmailID = representativeDTO.EmailID,
                Rating = representativeDTO.Rating,
            };

            return View(representative);
        }

        [Authorize(Roles = "Admin, Representative")]
        // GET: Representatives/Create
        public async Task<IActionResult> Create()
        {
            await Check();

            List<Orphanage> orphanages = await _unitOfWorkAsync.Orphanages.GetAll()
                .OrderBy(o => o.Name).ToListAsync();

            ViewBag.Orphanages = new SelectList(orphanages, "ID", "Name");

            return View();
        }



        // POST: Representatives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Representative")]
        public async Task<IActionResult> Create([Bind("Name,Surname,Patronymic,Birthday,Rating,Avatar,ChildrenHouseID")]
        RepresentativeDTO representativeDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(representativeDTO);
            }

            Stream stream = null;

            if (representativeDTO.Avatar != null)
            {
                stream = _streamCreater.CopyFileToStream(representativeDTO.Avatar);
            }

            var url = _URLRepresentativeBuilder.CreatePost(_apiPath);
            var status = await _downLoader.CreatePostAsync(url, representativeDTO,
                                                 stream,
                                                 representativeDTO.Avatar.FileName);

            if (status != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Representatives/Edit/5
        [Authorize(Roles = "Admin, Representative")]
        public async Task<IActionResult> Edit(int? id)
        {

            List<Orphanage> orphanages = _unitOfWorkAsync.Orphanages.GetAll()
                .OrderBy(o => o.Name).ToList();
            ViewBag.Orphanages = new SelectList(orphanages, "ID", "Name");


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

            var url = _URLRepresentativeBuilder.GetById(_apiPath, id.Value);
            RepresentativeDTO representativeDTO = null;

            try
            {
                representativeDTO = await _downLoader.GetByIdAsync(url);
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

            return View(representativeDTO);
        }

        // POST: Representatives/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Representative")]
        public async Task<IActionResult> Edit(int id, RepresentativeDTO representativeDTO)
        {
            if (id != representativeDTO.ID)
            {
                return NotFound();
            }

            Stream stream = null;

            if (representativeDTO.Avatar != null)
            {
                stream = _streamCreater.CopyFileToStream(representativeDTO.Avatar);
            }

            var url = _URLRepresentativeBuilder.GetById(_apiPath, id);
            var status = await _downLoader.CreatePutAsync(url, representativeDTO,
                                                            stream, representativeDTO.Avatar?.FileName);

            if (status != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Representatives/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLRepresentativeBuilder.GetById(_apiPath, id.Value);
            RepresentativeDTO representativeDTO = null;

            try
            {
                representativeDTO = await _downLoader.GetByIdAsync(url);
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

            if (representativeDTO == null)
            {
                return NotFound();
            }

            return View(representativeDTO);
        }

        // POST: Representatives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var url = _URLRepresentativeBuilder.GetById(_apiPath, id);
            var status = await _downLoader.DeleteAsync(url);

            if (status != HttpStatusCode.OK)
            {
                return Redirect("Home/Error");
            }

                return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}