using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
using FamilyNet.Models.Identity;
namespace FamilyNet.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : BaseController
    {
        ServerSimpleDataDownloader<RoleDTO> _downloader;
        private readonly string _apiPath = "http://localhost:53605/api/v1/roles/";

        public RolesController(IUnitOfWorkAsync unitOfWork, ServerSimpleDataDownloader<RoleDTO> downloader) : base(unitOfWork)
        {
            _downloader = downloader;
        }
        public async Task<IActionResult> Index()
        {
            var url = _apiPath;
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
          

            return View(roles);
        }
        
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(RoleDTO role)
        {

            if (!ModelState.IsValid)
            {
                return View(role);
            }

            var url = _apiPath;
            var status = await _downloader.CreatePostAsync(url, role, HttpContext.Session);

            if (status.StatusCode != HttpStatusCode.Created)
            {
                return Redirect("/Home/Error");
                //TODO: log
            }

            return Redirect("/Roles/Index");
        
        }

        
        public async Task<IActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var url = _apiPath+id;
            
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

            return RedirectToAction("Index");
        }

        public IActionResult UserList() => View(_unitOfWork.UserManager.Users);

        public async Task<IActionResult> Edit(string userId)
        {
            // получаем пользователя
            ApplicationUser user = await _unitOfWork.UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
                var allRoles = _unitOfWork.RoleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            // получаем пользователя
            ApplicationUser user = await _unitOfWork.UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
                // получаем все роли
                var allRoles = _unitOfWork.RoleManager.Roles.ToList();
                // получаем список ролей, которые были добавлены
                var addedRoles = roles.Except(userRoles);
                // получаем роли, которые были удалены
                var removedRoles = userRoles.Except(roles);

                await _unitOfWork.UserManager.AddToRolesAsync(user, addedRoles);

                await _unitOfWork.UserManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("UserList");
            }

            return NotFound();
        }

    }
}
