using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models.ViewModels;
using FamilyNet.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using FamilyNet.Downloader;
using DataTransferObjects;
using Microsoft.AspNetCore.Http;

namespace FamilyNet.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IAuthorizeCreater _authorizeCreater;
        private readonly string _headerToken = "Bearer";

        public AccountController(IUnitOfWorkAsync unitOfWork,
                                 IStringLocalizer<HomeController> localizer,
                                 IStringLocalizer<SharedResource> sharedLocalizer,
                                 IAuthorizeCreater authorizeCreater)
            : base(unitOfWork, sharedLocalizer)
        {
            _localizer = localizer;
            _authorizeCreater = authorizeCreater;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            GetViewData();
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
            GetViewData();
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
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _unitOfWorkAsync.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _unitOfWorkAsync.UserManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                await _unitOfWorkAsync.SignInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
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
            GetViewData();
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            GetViewData();

            if (ModelState.IsValid)
            {
                var dto = new CredentialsDTO() { Email = model.Email, Password = model.Password };
                var token = await _authorizeCreater.Login(dto);

                if (!String.IsNullOrEmpty(token))
                {
                    HttpContext.Session.SetString(_headerToken, token);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.SetString(_headerToken, String.Empty);
            GetViewData();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            GetViewData();
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
            if (GetCurrentUserAsync().Result.PersonType == PersonType.User)
            {
                RedirectToAction("Index", "Home");
            }
            if (!GetCurrentUserAsync().Result.HasPerson)
            {
                return GetRedirect(GetCurrentUserAsync().Result.PersonType.ToString(), "Create");
            }
            return View();
        }

        private IActionResult GetRedirect(string role, string action)
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

        private void GetViewData()
        {
            ViewData["CharityMakers"] = _localizer["CharityMakers"];
        }
    }
}
