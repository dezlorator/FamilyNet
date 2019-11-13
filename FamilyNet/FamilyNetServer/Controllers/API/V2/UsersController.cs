using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FamilyNetServer.Models.Identity;
using System.Linq;
using DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using FamilyNetServer.HttpHandlers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FamilyNetServer.Controllers.API.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsersController> _logger;
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public UsersController(IUnitOfWork unitOfWork,
                               ILogger<UsersController> logger,
                               IIdentityExtractor identityExtractor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _identityExtractor = identityExtractor;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin, Orphan, Volunteer, CharityMaker, Representative")]
        public IActionResult Get()
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
               "Endpoint Users/api/v2 GetAll was called", userId, token);

            var users = _unitOfWork.UserManager.Users;

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(users.ToList()));

            return Ok(users);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin, Orphan, Volunteer, CharityMaker, Representative")]
        public async Task<IActionResult> GetAsync(string id)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Users/api/v2 GetAsync was called", userId, token);

            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
            var allRoles = _unitOfWork.RoleManager.Roles.ToArray();

            if (user == null)
            {
                _logger.LogError("{info}{status}",
                    $"User wasn't found [id:{id}]",
                    StatusCodes.Status400BadRequest);

                return BadRequest();
            }

            var userDTO = new UserDTO()
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = userRoles.ToList()
            };

            _logger.LogInformation("{status} {json}", StatusCodes.Status200OK,
                JsonConvert.SerializeObject(userDTO));

            return Ok(userDTO);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody]UserDTO userDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info} {userId} {token}",
                "Endpoint Users/api/v2 [POST] was called", userId, token);

            if (userDTO == null)
            {
                _logger.LogWarning("{status}{token}{userId}{info}",
                    StatusCodes.Status400BadRequest, token, userId,
                    "UserDTO is null");

                return BadRequest();
            }

            var user = new ApplicationUser
            {
                Email = userDTO.Email,
                UserName = userDTO.Email,
                PhoneNumber = userDTO.PhoneNumber
            };

            var result = await _unitOfWork.UserManager.CreateAsync(user, userDTO.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("{token}{userId}{status}{info}",
                    token, userId, StatusCodes.Status201Created,
                    $"User was saved [id:{user.Id}]");

                _unitOfWork.SaveChanges();

                return Created("api/v2/users", userDTO);
            }

            _logger.LogWarning("{status}{token}{userId}{info}",
                StatusCodes.Status400BadRequest, token, userId,
                "User was not created");

            return BadRequest();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Users/api/v2 [DELETE] was called", userId, token);

            var user = await _unitOfWork.UserManager.FindByIdAsync(id);

            if (user == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"User was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            var result = await _unitOfWork.UserManager.DeleteAsync(user);
            _unitOfWork.SaveChanges();

            _logger.LogInformation("{status} {info} {userId} {token}",
                StatusCodes.Status200OK, $"User was deleted [id:{id}]",
                userId, token);

            return Ok();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin, Orphan, Volunteer, CharityMaker, Representative")]
        public async Task<IActionResult> EditAsync(string id, UserDTO us)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Users/api/v2 [PUT] was called", userId, token);

            var user = await _unitOfWork.UserManager.FindByIdAsync(us.Id);

            if (user == null)
            {
                _logger.LogError("{status} {info} {userId} {token}",
                    StatusCodes.Status400BadRequest,
                    $"User was not found [id:{id}]", userId, token);

                return BadRequest();
            }

            user.Email = us.Email;
            user.UserName = us.Email;

            var validEmail = await _unitOfWork.UserValidator
                .ValidateAsync(_unitOfWork.UserManager, user);

            if (!validEmail.Succeeded)
            {
                _logger.LogError("{userId} {token} {status} {info}",
                    userId, token, StatusCodes.Status400BadRequest,
                    "User's email is invalid");

                return BadRequest();
            }

            user.PhoneNumber = us.PhoneNumber;
            var validPhone = await _unitOfWork.PhoneValidator
                .ValidateAsync(_unitOfWork.UserManager, user);

            if (!validPhone.Succeeded)
            {
                _logger.LogError("{userId} {token} {status} {info}",
                    userId, token, StatusCodes.Status400BadRequest,
                    "User's phone is invalid");

                return BadRequest();
            }

            IdentityResult validPass = null;

            if (!string.IsNullOrEmpty(us.Password))
            {
                validPass = await _unitOfWork.PasswordValidator
                    .ValidateAsync(_unitOfWork.UserManager, user, us.Password);

                if (validPass.Succeeded)
                {
                    user.PasswordHash = _unitOfWork
                        .PasswordHasher.HashPassword(user, us.Password);
                }
                else
                {
                    _logger.LogError("{userId} {token} {status} {info}",
                        userId, token, StatusCodes.Status400BadRequest,
                        "User's password is invalid");

                    return BadRequest();
                }
            }

            var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
            var allRoles = _unitOfWork.RoleManager.Roles.ToList();
            var addedRoles = us.Roles.Except(userRoles);
            var removedRoles = userRoles.Except(us.Roles);
            await _unitOfWork.UserManager.AddToRolesAsync(user, addedRoles);
            await _unitOfWork.UserManager.RemoveFromRolesAsync(user, removedRoles);

            var result = await _unitOfWork.UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("{token}{userId}{status}{info}",
                    token, userId, StatusCodes.Status204NoContent,
                    $"User was updated [id:{user.Id}]");

                return NoContent();
            }

            _logger.LogError("{status} {info} {userId} {token}",
                StatusCodes.Status400BadRequest,
                $"User was not updated [id:{id}]", userId, token);

            return BadRequest();
        }
    }
}








