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

namespace FamilyNetServer.Controllers.API.V2
{
    [Route("api/v2/[controller]")]
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
                    EmailConfirmed = true,
                    PersonID = null
                };

                var result = await _unitOfWork.UserManager
                    .CreateAsync(user, model.Password);

                await _unitOfWork.UserManager.AddToRoleAsync(user,
                    model.YourDropdownSelectedValue);

                if (result.Succeeded)
                {
                    return Ok(model);
                }


                var msg = "model is not valid";
                _logger.LogError("{info}{status}", msg,
                    StatusCodes.Status400BadRequest);
                return BadRequest(msg);
            }
            return BadRequest();
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