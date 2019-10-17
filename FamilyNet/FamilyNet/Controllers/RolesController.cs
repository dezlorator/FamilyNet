using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using FamilyNet.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Models.Interfaces;

namespace FamilyNet.Controllers
{
    /// <summary>
    /// Controller for managing roles. For Admin only
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class RolesController : BaseController
    {
        /// <summary>
        /// Construtor  RolesController
        /// </summary>
        /// <param name="unitOfWork"></param>
        public RolesController(IUnitOfWorkAsync unitOfWork) : base(unitOfWork)
        {

        }

        /// <summary>
        ///Creates view index page
        /// </summary>
        /// <returns>view index page</returns>
        public IActionResult Index() => View(_unitOfWorkAsync.RoleManager.Roles);

        /// <summary>
        /// Creates View Create Role Page
        /// </summary>
        /// <returns></returns>
        public IActionResult Create() => View();

        /// <summary>
        /// Create new role and add to database
        /// </summary>
        /// <param name="name">role name</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _unitOfWorkAsync.RoleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(name);
        }
        /// <summary>
        /// Delete role frome database
        /// </summary>
        /// <param name="id">role id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _unitOfWorkAsync.RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await _unitOfWorkAsync.RoleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Creates View for Users
        /// </summary>
        /// <returns></returns>
        public IActionResult UserList() => View(_unitOfWorkAsync.UserManager.Users);

        /// <summary>
        /// Creates page for editing user roles 
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(string userId)
        {
            // получаем пользователя
            ApplicationUser user = await _unitOfWorkAsync.UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _unitOfWorkAsync.UserManager.GetRolesAsync(user);
                var allRoles = _unitOfWorkAsync.RoleManager.Roles.ToList();
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
        /// <summary>
        /// Saves edited changes to database
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="roles">list of roles</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            // получаем пользователя
            ApplicationUser user = await _unitOfWorkAsync.UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                // получем список ролей пользователя
                var userRoles = await _unitOfWorkAsync.UserManager.GetRolesAsync(user);
                // получаем все роли
                var allRoles = _unitOfWorkAsync.RoleManager.Roles.ToList();
                // получаем список ролей, которые были добавлены
                var addedRoles = roles.Except(userRoles);
                // получаем роли, которые были удалены
                var removedRoles = userRoles.Except(roles);

                await _unitOfWorkAsync.UserManager.AddToRolesAsync(user, addedRoles);

                await _unitOfWorkAsync.UserManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("UserList");
            }

            return NotFound();
        }

    }
}
