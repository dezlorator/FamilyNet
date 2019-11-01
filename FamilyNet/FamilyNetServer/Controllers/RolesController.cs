using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FamilyNetServer.Models;
using FamilyNetServer.Models.ViewModels;
using FamilyNetServer.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using FamilyNetServer.Models.Interfaces;

namespace FamilyNetServer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : BaseController
    {

        public RolesController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        public IActionResult Index() => View(_unitOfWork.RoleManager.Roles);

        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IdentityResult result = await _unitOfWork.RoleManager.CreateAsync(new IdentityRole(name));
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

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _unitOfWork.RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await _unitOfWork.RoleManager.DeleteAsync(role);
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
