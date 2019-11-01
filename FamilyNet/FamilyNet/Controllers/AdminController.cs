using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using FamilyNet.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Infrastructure;
using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;
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
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        ServerSimpleDataDownloader<UserDTO> _downloader;
        ServerSimpleDataDownloader<RoleDTO> _rolesDownloader;
        private readonly string _apiPath = "http://localhost:53605/api/v1/users/";

        private readonly string _apiRolesPath = "http://localhost:53605/api/v1/roles/";
        public AdminController(IUnitOfWorkAsync unitOfWork, ServerSimpleDataDownloader<UserDTO> downloader
            , ServerSimpleDataDownloader<RoleDTO> rolesDownloader)
                              : base(unitOfWork)
        {
            _downloader = downloader;
            _rolesDownloader = rolesDownloader;
        }

        public async Task<IActionResult> Index()
        {
            var url = _apiPath;
            IEnumerable<UserDTO> userDTO = null;

            try
            {
                userDTO = await _downloader.GetAllAsync(url);
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



            var users = userDTO.Select(user => new ApplicationUser()
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            });


            return View(users);
        }
        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }
            var url = _apiPath;
            var status = await _downloader.CreatePostAsync(url, model);

            if (status.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            return Redirect("/Admin/Index");


        }

        public async Task<IActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _apiPath + id;

            try
            {
                var status = await _downloader.DeleteAsync(url);

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

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(string id)
        {
           if (id == null)
            {
                return NotFound();
            }
            string url = _apiPath + id;
            UserDTO userDTO = new UserDTO();
            try
            {
                var allRoles = await _rolesDownloader.GetAllAsync(_apiRolesPath);
                userDTO = await _downloader.GetByIdAsync(url);
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

            
            var url = _apiPath+id;

            try
            {
               var status = await _downloader.CreatePutAsync(url, userDTO);
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

            return Redirect("/Admin/Index");
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        public IActionResult SeedData()
        {
            SeedData seedData = new SeedData(_unitOfWorkAsync);
            seedData.EnsurePopulated();

            return Redirect("/Home/Index");
        }
    }
}
