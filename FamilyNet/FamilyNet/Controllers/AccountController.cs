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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FamilyNet.Controllers
{
    
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWorkAsync unitOfWork) : base(unitOfWork)
        {
          
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var allRoles = _unitOfWorkAsync.RoleManager.Roles.ToList();
            var yourDropdownList = new SelectList(allRoles.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Name
            }).ToList(), "Value", "Text");
            var viewModel = new RegisterViewModel()
            {
                // The Dropdownlist values
                YourDropdownList = yourDropdownList
            };
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var allRoles = _unitOfWorkAsync.RoleManager.Roles.ToList();
            var yourDropdownList = new SelectList(allRoles.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Name
            }).ToList(), "Value", "Text");
            model.YourDropdownList = yourDropdownList;
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.Phone,
                    PersonType = GetPersonType(model.YourDropdownSelectedValue),
                    PersonID = null
                };
                // добавляем пользователя.
                var result = await _unitOfWorkAsync.UserManager.CreateAsync(user, model.Password);

                await _unitOfWorkAsync.UserManager.AddToRoleAsync(user, model.YourDropdownSelectedValue);

                if (result.Succeeded)
                {

                    var code = await _unitOfWorkAsync.UserManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, code = code },
                        protocol: HttpContext.Request.Scheme);
                    EmailService emailService = new EmailService();
                    await emailService.SendEmailAsync(model.Email, "Confirm your account",
                        $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");


                    return Content("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
                    //await _unitOfWorkAsync.SignInManager.SignInAsync(user, false);
                    //return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
            
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if(userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _unitOfWorkAsync.UserManager.FindByIdAsync(userId);
            if(user == null)
            {
                return View("Error");
            }
            var result = await _unitOfWorkAsync.UserManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return GetRedirect(model.YourDropdownSelectedValue, "Create");
            }
            else
            {
                return View("Error");
            }
        }
       

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _unitOfWorkAsync.UserManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if(!await _unitOfWorkAsync.UserManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError(string.Empty, "Вы не подтвердили свой email");
                        return View(model);
                    }
                    await _unitOfWorkAsync.SignInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result =
                            await _unitOfWorkAsync.SignInManager.PasswordSignInAsync(
                                user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Такого пользователя не существует, зарегистрируйтесь, пожалуйста!");
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _unitOfWorkAsync.SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult GetDetails()
        {
            var url = Url.Action("Details", GetCurrentUserAsync().Result.PersonType.ToString() + "s", new { id = GetCurrentUserAsync().Result.PersonID });
            return Redirect(url);            
        }

        public IActionResult AccountEdits()
        {
            var url = Url.Action("Edit", GetCurrentUserAsync().Result.PersonType.ToString() + "s", new { id = GetCurrentUserAsync().Result.PersonID });
            return Redirect(url);
        }

        public IActionResult PersonalRoom()
        {
            return View();
        }

        private IActionResult GetRedirect(string role , string action)
        {
            switch (role)
            {
                case "CharityMaker":
                    return RedirectToAction(action, "CharityMakers");
                case "Orphan":
                    return RedirectToAction(action, "Orphans");
                case "Representative":
                    return RedirectToAction(action, "Representatives");
                case "Volunteer":
                    return RedirectToAction(action, "Volunteers");
                default:
                    return RedirectToAction(action, "Index");
            }
        }


        private static PersonType GetPersonType(string role)
        {
            switch (role)
            {
                case "CharityMaker":
                    return PersonType.CharityMaker;
                case "Representative":
                    return PersonType.Representative;
                case "Volunteer":
                    return PersonType.Volunteer;
                case "Orphan":
                    return PersonType.Orphan;
                case "User":
                    return PersonType.User;
                default:
                    return PersonType.User;
            }
        }
    }
}
