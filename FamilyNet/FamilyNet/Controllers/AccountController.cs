using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNet.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using FamilyNet.Downloader;
using DataTransferObjects;
using Microsoft.AspNetCore.Http;
using FamilyNet.IdentityHelpers;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using FamilyNet.Enums;

namespace FamilyNet.Controllers
{
    public class AccountController : Controller
    {
        #region private fields

        private readonly IIdentityInformationExtractor _identityInformationExtactor;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IAuthorizeCreater _authorizeCreater;
        private readonly string _headerToken = "Bearer";

        private readonly ServerSimpleDataDownloader<RegistrationDTO> _registrationDownloader;
        private readonly IURLRegistrationBuilder _registrationBuilder;
        private readonly string _apiRegistrationPath = "api/v1/registration";

        private readonly ServerSimpleDataDownloader<RoleDTO> _rolesDownloader;
        private readonly IURLRolesBuilder _rolesBuilder;
        private readonly string _apiRolesPath = "api/v1/roles";

        #endregion

        #region ctor

        public AccountController(IStringLocalizer<HomeController> localizer,
                                IStringLocalizer<SharedResource> sharedLocalizer,
                                IAuthorizeCreater authorizeCreater,
                                IIdentityInformationExtractor identityInformationExtactor,
                                ServerSimpleDataDownloader<RegistrationDTO> registrationDownloader,
                                ServerSimpleDataDownloader<RoleDTO> rolesDownloader,
                                IURLRegistrationBuilder registrationBuilder,
                                IURLRolesBuilder rolesBuilder)
        {
            _localizer = localizer;
            _authorizeCreater = authorizeCreater;
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
                    HttpContext.Session.SetString("id", result.Token.Id);
                    HttpContext.Session.SetString("email", result.Token.Email);
                    HttpContext.Session.SetString("roles", String.Join(",", result.Token.Roles));

                    if (result.Token.PersonId != null)
                    {
                        HttpContext.Session.SetString("personId", result.Token.PersonId.ToString());
                    }

                    HttpContext.Session.SetString(_headerToken, result.Token.Token);

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
            var id = HttpContext.Session.GetString("id");
            var role = HttpContext.Session.GetString("roles");
            var personId = HttpContext.Session.GetString("personId");
            var url = Url.Action("Details", role + "s", new { id = personId });

            GetViewData();
            return Redirect(url);
        }

        public IActionResult AccountEdits()
        {
            var personId = HttpContext.Session.GetString("personId");
            var role = HttpContext.Session.GetString("roles");
            var url = Url.Action("Edit", role + "s", new { id = personId });
            return Redirect(url);
        }

        public IActionResult PersonalRoom()
        {
            var role = HttpContext.Session.GetString("roles");
            if (GetPersonType(role) == PersonType.User)
            {
                RedirectToAction("Index", "Home");
            }
            var personId = HttpContext.Session.GetString("personId");
            if (GetPersonType(role) != PersonType.User && (personId == String.Empty || personId == null))
            {
                var url = Url.Action("Create", role + "s");
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
