using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FamilyNetServer.Models.ViewModels;
using FamilyNetServer.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using DataTransferObjects;
using Microsoft.AspNetCore.Http;
using DataTransferObjects.Enums;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        #region private fields
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        public RegistrationController(IUnitOfWork unitOfWork, IStringLocalizer<HomeController> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Register()
        {
          
            var allRoles = _unitOfWork.RoleManager.Roles.ToList();
            var yourDropdownList = new SelectList(allRoles.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Name
            }).ToList(), "Value", "Text");
            var viewModel = new RegisterViewModel()
            {
                YourDropdownList = yourDropdownList
            };
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromForm]RegistrationDTO model)
        {         
            var allRoles = _unitOfWork.RoleManager.Roles.ToList();
            var yourDropdownList = new SelectList(allRoles.Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Name
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

                    var codeTokken = await _unitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, code = codeTokken },
                        protocol: HttpContext.Request.Scheme);
                    EmailService emailService = new EmailService();
                    await emailService.SendEmailAsync(model.Email, "Confirm your account",
                        $"Confirm regisration by link: <a href='{callbackUrl}'>link</a>");

                    return Ok(model);
                    //return Content("To finish registration check your email and click on the link in the letter.");
                }

                else
                {
                    string e=string.Empty;
                    foreach (var error in result.Errors)
                    {
                        e += error.Description;
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(e);
                }
                
            }
            return BadRequest("model is not valid");
           

        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }
            var user = await _unitOfWork.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }
            var result = await _unitOfWork.UserManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                await _unitOfWork.SignInManager.SignInAsync(user, false);
                return Ok();
            }
            else
            {
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