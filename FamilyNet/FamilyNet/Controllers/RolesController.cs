using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using DataTransferObjects;
using FamilyNet.Downloader;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using FamilyNet.IdentityHelpers;

namespace FamilyNet.Controllers
{
    public class RolesController : Controller
    {
        #region fields

        private readonly ServerSimpleDataDownloader<RoleDTO> _downloader;
        private readonly IURLRolesBuilder _URLRolesBuilder;
        private readonly string _apiPath = "api/v1/roles";

        private readonly IIdentityInformationExtractor _identityInformationExtactor;

        #endregion

        #region ctor

        public RolesController(ServerSimpleDataDownloader<RoleDTO> downloader,
                               IIdentityInformationExtractor identityInformationExtactor,
                               IURLRolesBuilder urlRolesBuilder)
        {
            _downloader = downloader;
            _identityInformationExtactor = identityInformationExtactor;
            _URLRolesBuilder = urlRolesBuilder;
        }

        #endregion

        #region methods

        public async Task<IActionResult> Index()
        {
            var url = _URLRolesBuilder.GetAll(_apiPath);
            IEnumerable<RoleDTO> rolesDTO = null;

            try
            {
                rolesDTO = await _downloader.GetAllAsync(url, HttpContext.Session);
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

            var roles = rolesDTO.Select(role => new IdentityRole()
            {   Id = role.ID,
                Name = role.Name
            });

            GetViewData();
            return View(roles);
        }

        public IActionResult Create() 
        {
            GetViewData();
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> Create(RoleDTO role)
        {
            if (!ModelState.IsValid)
            {
                return View(role);
            }

            var url = _URLRolesBuilder.CreatePost(_apiPath);
            var status = await _downloader.CreatePostAsync(url, role, HttpContext.Session);

            if (status.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/Account/Login");
            }

            if (status.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            GetViewData();

            return Redirect("/Roles/Index");
        }
        
        public async Task<IActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url =_URLRolesBuilder.Delete(_apiPath,id);
            
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

        private void GetViewData()
        {
            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                            ViewData);
        }

        #endregion
    }
}
