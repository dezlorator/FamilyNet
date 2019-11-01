using FamilyNetServer.DTO;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using FamilyNetServer.Models.Identity;
using System.Linq;



namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly ILogger<UsersController> _logger;

        #endregion

        public UsersController(IUnitOfWorkAsync unitOfWork, ILogger<UsersController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get()
        {
            var users = _unitOfWork.UserManager.Users;
            if (users==null)
            {
                _logger.LogError("BadRequest[400]. List of users was not fount.");
                return BadRequest();
            }
            _logger.LogInformation("0k[200]. List of users was sent.");
            return Ok(users);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAsync(string id)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
            var allRoles = _unitOfWork.RoleManager.Roles.ToArray();

            if (user == null)
            {
                _logger.LogError("BadRequest[400]. User was not fount.");
                return BadRequest();
            }

            var userDTO = new UserDTO()
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = userRoles.ToList()
            };
            _logger.LogInformation("0k[200]. User was sent.");
            return Ok(userDTO);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromForm]UserDTO userDTO)
        {
            if (userDTO == null)
            {
                _logger.LogError("BadRequest[400]. UserDTO is not valid, user vas not created.");
                return BadRequest();
            }
            var user = new ApplicationUser
            {
                Email = userDTO.Email,
                UserName = userDTO.Email,
                PhoneNumber = userDTO.PhoneNumber
            };
            IdentityResult result
                       = await _unitOfWork.UserManager.CreateAsync(user, userDTO.Password);
            if (result.Succeeded)
            {
                _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Created[201].New user was created.");
                return Created("api/v1/users/", userDTO);
            }
            _logger.LogError("BadRequest[400]. UserDTO is not valid, user vas not created.");
            return BadRequest();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            ApplicationUser user = await _unitOfWork.UserManager.FindByIdAsync(id);

            if (user == null)
            {
                _logger.LogError("BadRequest[400]. Users was not found and not deleted.");
                return BadRequest();
            }

            IdentityResult result
                       = await _unitOfWork.UserManager.DeleteAsync(user);
            _logger.LogInformation("0k[200]. User was deleted.");
            _unitOfWork.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditAsync(string id, UserDTO us)
        {
            ApplicationUser user = await _unitOfWork.UserManager.FindByIdAsync(us.Id);
            if (user == null)
            {
                _logger.LogError("BadRequest[400]. User was ot found, user vas not created.");
                return BadRequest();
            }

            //validate email
            user.Email = us.Email;
            user.UserName = us.Email;
            IdentityResult validEmail
                = await _unitOfWork.UserValidator.ValidateAsync(_unitOfWork.UserManager, user);
            if (!validEmail.Succeeded)
            {
                _logger.LogError("BadRequest[400]. User EMAIL is not valid, user vas not created.");
                return BadRequest();
            }

            //validate phone
            user.PhoneNumber = us.PhoneNumber;
            IdentityResult validPhone
                = await _unitOfWork.PhoneValidator.ValidateAsync(_unitOfWork.UserManager, user);
            if (!validPhone.Succeeded)
            {
                _logger.LogError("BadRequest[400]. User Phone is not valid, user vas not created.");
                return BadRequest();
            }

            //validate password
            IdentityResult validPass = null;
            if (!string.IsNullOrEmpty(us.Password))
            {
                validPass = await _unitOfWork.PasswordValidator.ValidateAsync(_unitOfWork.UserManager,
                    user, us.Password);
                if (validPass.Succeeded)
                {
                    user.PasswordHash = _unitOfWork.PasswordHasher.HashPassword(user,
                        us.Password);
                }
                else
                {
                    _logger.LogError("BadRequest[400]. User PASSWORD is not valid, user vas not created.");
                    return BadRequest();
                }
            }

            // получем список ролей пользователя
            var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
            // получаем список ролей, которые были добавлены
            var addedRoles = us.Roles.Except(userRoles);
            // получаем роли, которые были удалены
            var removedRoles = userRoles.Except(us.Roles);

            await _unitOfWork.UserManager.AddToRolesAsync(user, addedRoles);

            await _unitOfWork.UserManager.RemoveFromRolesAsync(user, removedRoles);

            IdentityResult result = await _unitOfWork.UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("0k[200]. User was edited.");
                return NoContent();
            }
            else
            {
                _logger.LogError("BadRequest[400]. User ROLES is not valid, user vas not created.");
                return BadRequest();
            }
        }
    }
}








