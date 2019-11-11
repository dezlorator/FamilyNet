using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNetServer.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using DataTransferObjects;
using DataTransferObjects.Enums;
using Microsoft.AspNetCore.Http;
using FamilyNetServer.HttpHandlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityExtractor _identityExtractor;
        private readonly ILogger<RegistrationController> _logger;
        #endregion

        #region ctor

        public RegistrationController(IUnitOfWork unitOfWork,
                                      ILogger<RegistrationController> logger,
                                      IIdentityExtractor identityExtractor)
        {
            _unitOfWork = unitOfWork;
            _identityExtractor = identityExtractor;
            _logger = logger;
        }

        #endregion

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromForm]RegistrationDTO model)
        {
            _logger.LogInformation("{info}",
                "Endpoint Registration/api/v1 [POST] was called");

            var allRoles = _unitOfWork.RoleManager.Roles.ToList();

            _logger.LogInformation("{json}{info}",
                JsonConvert.SerializeObject(allRoles), "json contains roles");

            var yourDropdownList = new SelectList(allRoles.Select(role =>
                new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Name
                }).ToList(), "Value", "Text");

            model.YourDropdownList = yourDropdownList;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.Phone,
                    PersonType = GetPersonType(model.YourDropdownSelectedValue),
                    PersonID = null
                };

                var result = await _unitOfWork.UserManager
                    .CreateAsync(user, model.Password);

                await _unitOfWork.UserManager.AddToRoleAsync(user,
                    model.YourDropdownSelectedValue);

                if (result.Succeeded)
                {
                    _logger.LogInformation("{info}", "User was saved");

                    var codeTokken = await _unitOfWork.UserManager
                        .GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ConfirmEmailAsync",
                        "Registration",
                        new { userId = user.Id, code = codeTokken },
                        protocol: HttpContext.Request.Scheme);
                    EmailService emailService = new EmailService();
                    await emailService.SendEmailAsync(model.Email,
                        "Confirm your account",
                        $"Confirm regisration by link: <a href='{callbackUrl}'>link</a>");

                    _logger.LogInformation("{info}{status}", "url " + callbackUrl,
                        StatusCodes.Status200OK);

                    return Ok(model);
                }
                else
                {
                    string e = string.Empty;

                    foreach (var error in result.Errors)
                    {
                        e += error.Description;
                        ModelState.AddModelError(string.Empty,
                            error.Description);
                    }

                    _logger.LogError("{info}{status}", "User was saved",
                        StatusCodes.Status400BadRequest);

                    return BadRequest(e);
                }
            }

            var msg = "model is not valid";
            _logger.LogError("{info}{status}", msg,
                StatusCodes.Status400BadRequest);

            return BadRequest(msg);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
        {
            _logger.LogInformation("{info}",
                "Endpoint Registration/api/v1 ConfirmEmailAsync was called");

            if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(code))
            {
                _logger.LogWarning("{info}{status}",
                    "Arguments are invalid", StatusCodes.Status400BadRequest);

                return BadRequest();
            }
            var user = await _unitOfWork.UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("{info}{status}",
                    "User was not found", StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var result = await _unitOfWork.UserManager
                .ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                await _unitOfWork.SignInManager.SignInAsync(user, false);

                _logger.LogInformation("{info}{status}",
                    "User's email confirmed", StatusCodes.Status200OK);

                return Ok();
            }
            else
            {
                _logger.LogError("{info}{status}",
                    "User's email isn't confirmed!",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
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