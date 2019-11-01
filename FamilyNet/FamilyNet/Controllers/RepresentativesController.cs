using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using FamilyNet.Downloader;
using FamilyNet.StreamCreater;
using DataTransferObjects;

namespace FamilyNet.Controllers
{
    public class RepresentativesController : Controller
    {
        #region private fields

        private readonly ServerDataDownloader<ChildrenHouseDTO> _childrenHouseDownloader;
        private readonly ServerDataDownloader<RepresentativeDTO> _representativesDownLoader;
        private readonly IURLRepresentativeBuilder _URLRepresentativeBuilder;
        private readonly IURLChildrenHouseBuilder _URLChildrenHouseBuilder;
        private readonly string _apiPath = "api/v1/representatives";
        private readonly string _apiChildrenHousesPath = "api/v1/childrenhouse";
        private readonly IFileStreamCreater _streamCreater;



        #endregion

        #region Ctor

        public RepresentativesController(IURLRepresentativeBuilder urlRepresentativeBuilder,
                                         IURLChildrenHouseBuilder URLChildrenHouseBuilder,
                                         ServerDataDownloader<RepresentativeDTO> downloader,
                                         ServerDataDownloader<ChildrenHouseDTO> childrenHouseDownloader,
                                         IFileStreamCreater streamCreater)
        {
            _URLRepresentativeBuilder = urlRepresentativeBuilder;
            _URLChildrenHouseBuilder = URLChildrenHouseBuilder;
            _representativesDownLoader = downloader;
            _childrenHouseDownloader = childrenHouseDownloader;
            _streamCreater = streamCreater;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Index(int id, 
            PersonSearchModel searchModel)
        {
            var url = _URLRepresentativeBuilder
                .GetAllWithFilter(_apiPath, searchModel, id);

            IEnumerable<RepresentativeDTO> representativesDTO = null;

            try
            {
                representativesDTO = await _representativesDownLoader.GetAllAsync(url, HttpContext.Session);
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
                representativeDTO = await _representativesDownLoader.GetByIdAsync(url, HttpContext.Session);
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

            return View();
        }



        // POST: Representatives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            var status = await _representativesDownLoader.CreatePostAsync(url, representativeDTO,
                                                 stream,
                                                 representativeDTO.Avatar.FileName,
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

       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _URLRepresentativeBuilder.GetById(_apiPath, id.Value);
            var urlChildrenHouse = _URLChildrenHouseBuilder
                .GetAllWithFilter(_apiChildrenHousesPath,
                                  new OrphanageSearchModel(),
                                  SortStateOrphanages.NameAsc);





            RepresentativeDTO representativeDTO = null;

            try
            {
                representativeDTO = await _representativesDownLoader.GetByIdAsync(url, HttpContext.Session);
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

            return View(representativeDTO);
        }

        // POST: Representatives/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            var status = await _representativesDownLoader.CreatePutAsync(url, representativeDTO,
                                                            stream, representativeDTO.Avatar?.FileName,
                                                            HttpContext.Session);

            if (status != HttpStatusCode.NoContent)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            return RedirectToAction(nameof(Index));
        }

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
                representativeDTO = await _representativesDownLoader.GetByIdAsync(url, HttpContext.Session);
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var url = _URLRepresentativeBuilder.GetById(_apiPath, id);
            var status = await _representativesDownLoader.DeleteAsync(url, HttpContext.Session);

            if (status != HttpStatusCode.OK)
            {
                return Redirect("Home/Error");
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}