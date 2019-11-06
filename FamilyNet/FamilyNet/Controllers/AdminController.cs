using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using DataTransferObjects;
using FamilyNet.Downloader;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using FamilyNet.IdentityHelpers;

namespace FamilyNet.Controllers
{
    public class AdminController : Controller
    {
        #region fields

        ServerSimpleDataDownloader<UserDTO> _downloader;
        private readonly IURLUsersBuilder _usersBuilder;
        private readonly string _apiUsersPath = "api/v1/users";

        ServerSimpleDataDownloader<RoleDTO> _rolesDownloader;
        private readonly IURLRolesBuilder _rolesBuilder;
        private readonly string _apiRolesPath = "api/v1/roles";

        private readonly IIdentityInformationExtractor _identityInformationExtactor;

        #endregion

        #region ctor

        public AdminController(ServerSimpleDataDownloader<UserDTO> downloader,
                              ServerSimpleDataDownloader<RoleDTO> rolesDownloader, 
                              IIdentityInformationExtractor identityInformationExtactor,
                              IURLRolesBuilder rolesBuilder, IURLUsersBuilder usersBuilder)
        {
            _downloader = downloader;
            _rolesDownloader = rolesDownloader;
            _identityInformationExtactor = identityInformationExtactor;
            _usersBuilder = usersBuilder;
            _rolesBuilder = rolesBuilder;
        }

        #endregion

        #region methods

        public async Task<IActionResult> Index()
        {
            var url = _usersBuilder.GetAll(_apiUsersPath);
            IEnumerable<UserDTO> userDTO = null;

            try
            {
                userDTO = await _downloader.GetAllAsync(url, HttpContext.Session);
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
            return View(userDTO);
        }
        public ViewResult Create()
        {
            GetViewData();
            return View();
        } 

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }
            var url = _usersBuilder.CreatePost(_apiUsersPath);
            var status = await _downloader.CreatePostAsync(url, model, HttpContext.Session);

            if (status.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            GetViewData();
            return Redirect("/Admin/Index");
        }

        public async Task<IActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _usersBuilder.GetById(_apiUsersPath, id);

            try
            {
                var status = await _downloader.DeleteAsync(url, HttpContext.Session);

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
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var url = _usersBuilder.GetById(_apiUsersPath, id);
            string urlRoles = _rolesBuilder.GetAll(_apiRolesPath);
            UserDTO userDTO = new UserDTO();
            try
            {
                var allRoles = await _rolesDownloader.GetAllAsync(urlRoles, HttpContext.Session);
                userDTO = await _downloader.GetByIdAsync(url, HttpContext.Session);
                ViewBag.AllRoles = allRoles.ToList();
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
            return View(userDTO);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserDTO userDTO, string id)
        {
            if (id != userDTO.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(userDTO);
            }

            var url = _usersBuilder.GetById(_apiUsersPath, id);

            try
            {
                var status = await _downloader.CreatePutAsync(url, userDTO, HttpContext.Session);
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
            return Redirect("/Admin/Index");
        }

        private void GetViewData()
        {
            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                           ViewData);
        }

        #endregion
    }
}