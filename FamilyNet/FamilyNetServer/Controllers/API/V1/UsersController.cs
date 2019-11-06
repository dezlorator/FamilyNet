using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FamilyNetServer.Models.Identity;
using System.Linq;
using DataTransferObjects;
using Microsoft.AspNetCore.Authorization;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region ctor

        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin, Orphan, Volunteer, CharityMaker, Representative")]
        public IActionResult Get()
        {
            var users = _unitOfWork.UserManager.Users;

            return Ok(users);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin, Orphan, Volunteer, CharityMaker, Representative")]
        public async Task<IActionResult> GetAsync(string id)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
            var allRoles = _unitOfWork.RoleManager.Roles.ToArray();

            if (user == null)
            {
                return BadRequest();
            }

            var userDTO = new UserDTO()
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = userRoles.ToList()
            };
            return Ok(userDTO);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromForm]UserDTO userDTO)
        {
            if (userDTO == null)
            {
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
                return Created("api/v1/users/", userDTO);
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            ApplicationUser user = await _unitOfWork.UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return BadRequest();
            }

            IdentityResult result
                       = await _unitOfWork.UserManager.DeleteAsync(user);

            _unitOfWork.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin, Orphan, Volunteer, CharityMaker, Representative")]
        public async Task<IActionResult> EditAsync(string id, UserDTO us)
        {
            ApplicationUser user = await _unitOfWork.UserManager.FindByIdAsync(us.Id);
            if (user == null)
            {
                return BadRequest();
            }

            //validate email
            user.Email = us.Email;
            user.UserName = us.Email;
            IdentityResult validEmail
                = await _unitOfWork.UserValidator.ValidateAsync(_unitOfWork.UserManager, user);
            if (!validEmail.Succeeded)
            {
                return BadRequest();
            }

            //validate phone
            user.PhoneNumber = us.PhoneNumber;
            IdentityResult validPhone
                = await _unitOfWork.PhoneValidator.ValidateAsync(_unitOfWork.UserManager, user);
            if (!validPhone.Succeeded)
            {
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
                    return BadRequest();
                }
            }

            // получем список ролей пользователя
            var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
            // получаем все роли
            var allRoles = _unitOfWork.RoleManager.Roles.ToList();
            // получаем список ролей, которые были добавлены
            var addedRoles = us.Roles.Except(userRoles);
            // получаем роли, которые были удалены
            var removedRoles = userRoles.Except(us.Roles);

            await _unitOfWork.UserManager.AddToRolesAsync(user, addedRoles);

            await _unitOfWork.UserManager.RemoveFromRolesAsync(user, removedRoles);

            IdentityResult result = await _unitOfWork.UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}








