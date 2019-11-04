using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models.ViewModels;
using FamilyNet.Models.Identity;
using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using FamilyNet.Downloader;
using DataTransferObjects;
using Microsoft.AspNetCore.Http;
using FamilyNet.Encoders;
using FamilyNet.IdentityHelpers;

namespace FamilyNet.Controllers
{
    public class AccountController : BaseController
    {
        #region private fields

        private readonly IIdentityInformationExtractor _identityInformationExtactor;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IAuthorizeCreater _authorizeCreater;
        private readonly string _headerToken = "Bearer";
        private readonly IJWTEncoder _encoder;

        #endregion

        #region ctor

        public AccountController(IIdentity unitOfWork,
                                IStringLocalizer<HomeController> localizer,
                                IStringLocalizer<SharedResource> sharedLocalizer,
                                IAuthorizeCreater authorizeCreater,
                                IJWTEncoder encoder,
                                IIdentityInformationExtractor identityInformationExtactor)
            : base(unitOfWork)
        {
            _localizer = localizer;
            _authorizeCreater = authorizeCreater;
            _encoder = encoder;
            _identityInformationExtactor = identityInformationExtactor;
        }

        #endregion

        [HttpGet]
        public IActionResult Register()
        {
            GetViewData();

            var allRoles = _unitOfWork.RoleManager.Roles.ToList();
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
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            GetViewData();
            var allRoles = _unitOfWork.RoleManager.Roles.ToList();
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

                var result = await _unitOfWork.UserManager.CreateAsync(user, model.Password);

                await _unitOfWork.UserManager.AddToRoleAsync(user, model.YourDropdownSelectedValue);

                if (result.Succeeded)
                {

                    var code = await _unitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(user);
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
            GetViewData();
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            GetViewData();
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _unitOfWork.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _unitOfWork.UserManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                await _unitOfWork.SignInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            GetViewData();
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            GetViewData();

            if (ModelState.IsValid)
            {
                var dto = new CredentialsDTO() { Email = model.Email, Password = model.Password };
                var result = await _authorizeCreater.Login(dto);

                if (result.Success)
                {
                    var claims = _encoder.GetTokenData(result.Token);
                    HttpContext.Session.SetString("id", claims.UserId.ToString());
                    HttpContext.Session.SetString("email", claims.Email);
                    HttpContext.Session.SetString("roles", String.Join(",", claims.Roles));
                    HttpContext.Session.SetString(_headerToken, result.Token);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            GetViewData();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
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

            GetViewData();
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

            GetViewData();

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
            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                           ViewData);
        }
    }
}
