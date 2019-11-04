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
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;


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

        private readonly ServerSimpleDataDownloader<RegistrationDTO> _registrationDownloader;
        private readonly IURLRegistrationBuilder _registrationBuilder;
        private readonly string _apiRegistrationPath = "api/v1/registration";

        private readonly ServerSimpleDataDownloader<RoleDTO> _rolesDownloader;
        private readonly IURLRolesBuilder _rolesBuilder;
        private readonly string _apiRolesPath = "api/v1/roles";

        #endregion

        #region ctor

        public AccountController(IIdentity unitOfWork,
                                IStringLocalizer<HomeController> localizer,
                                IStringLocalizer<SharedResource> sharedLocalizer,
                                IAuthorizeCreater authorizeCreater,
                                IJWTEncoder encoder,
                                IIdentityInformationExtractor identityInformationExtactor,
                                ServerSimpleDataDownloader<RegistrationDTO> registrationDownloader,
                                ServerSimpleDataDownloader<RoleDTO> rolesDownloader,
                                IURLRegistrationBuilder registrationBuilder,
                                IURLRolesBuilder rolesBuilder)
            : base(unitOfWork)
        {
            _localizer = localizer;
            _authorizeCreater = authorizeCreater;
            _encoder = encoder;
            _identityInformationExtactor = identityInformationExtactor;
            _registrationDownloader = registrationDownloader;
            _rolesDownloader = rolesDownloader;
            _rolesBuilder = rolesBuilder;
            _registrationBuilder = registrationBuilder;
        }

        #endregion
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            
            var urlRoles = _rolesBuilder.GetAll(_apiRolesPath);
            IEnumerable<RoleDTO> roles = null;

            roles = await _rolesDownloader.GetAllAsync(urlRoles, HttpContext.Session);

            var yourDropdownList = new SelectList(roles.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Name
            }).ToList(), "Value", "Text");
            var viewModel = new RegistrationDTO()
            {
                // The Dropdownlist values
                YourDropdownList = yourDropdownList
            };
            GetViewData();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationDTO model)
        {

            GetViewData();

            var urlRoles = _rolesBuilder.GetAll(_apiRolesPath);
            IEnumerable<RoleDTO> rolesDTO = null;

            rolesDTO = await _rolesDownloader.GetAllAsync(urlRoles, HttpContext.Session);
            var url = _registrationBuilder.Register(_apiRegistrationPath);
            var yourDropdownList = new SelectList(rolesDTO.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Name
            }).ToList(), "Value", "Text");

            model.YourDropdownList = yourDropdownList;
            if (ModelState.IsValid)
            {
                try
                {
                    var result = _registrationDownloader.CreatePostAsync(url, model, HttpContext.Session);
                    return Content("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
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
            }
            return View(model);


        }

        //[HttpGet]
        //public async Task<IActionResult> ConfirmEmail(string userId, string code)
        //{
        //    GetViewData();
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var user = await _unitOfWork.UserManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await _unitOfWork.UserManager.ConfirmEmailAsync(user, code);
        //    if (result.Succeeded)
        //    {
        //        await _unitOfWork.SignInManager.SignInAsync(user, false);
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //    {
        //        return View("Error");
        //    }
        //}

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
            var id =HttpContext.Session.GetString("id");
            var role = HttpContext.Session.GetString("roles");
            var url = Url.Action("Details", role + "s", new { id = GetCurrentUserAsync().Result.PersonID });

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
            var role = HttpContext.Session.GetString("roles");
            if (GetPersonType(role) == PersonType.User)
            {
                RedirectToAction("Index", "Home");
            }
            if (GetPersonType(role) != PersonType.User)
            {
                var url = Url.Action(role + "s", "Create");
                return Redirect(url);
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
            _identityInformationExtactor.GetUserInformation(HttpContext.Session,
                                                           ViewData);
        }
    }
}
