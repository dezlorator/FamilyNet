using FamilyNetServer.DTO;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using FamilyNetServer.Models.ViewModels;
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

        #endregion

        public UsersController(IUnitOfWorkAsync unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get()
        {
            var users = _unitOfWork.UserManager.Users;

            return Ok(users);
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
                UserName = userDTO.UserName,
                PhoneNumber = userDTO.PhoneNumber
            };

            await _unitOfWork.UserManager.CreateAsync(user, userDTO.Password);

            _unitOfWork.SaveChangesAsync();


            return Created("api/v1/users/", userDTO);

        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            ApplicationUser user = await _unitOfWork.UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return BadRequest();
            }

            await _unitOfWork.UserManager.DeleteAsync(user);

            _unitOfWork.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditAsync(UserDTO userDTO)
        {
            ApplicationUser user = await _unitOfWork.UserManager.FindByIdAsync(userDTO.Id);

            if (user == null)
            {
                return BadRequest();
            }

            //IdentityResult validEmail
            //      = await _unitOfWork.UserValidator.ValidateAsync(_unitOfWork.UserManager, user);
            //IdentityResult validPhone
            // = await _unitOfWork.PhoneValidator.ValidateAsync(_unitOfWork.UserManager, user);
            //IdentityResult validPass = await _unitOfWork.PasswordValidator.ValidateAsync(_unitOfWork.UserManager,
            //           user, userDTO.Password);
            //if (!validEmail.Succeeded || !validPhone.Succeeded /*|| !validPass.Succeeded*/)
            //{
            //    return BadRequest();
            //}

            user.UserName = userDTO.UserName;
            user.Email = userDTO.Email;
            user.PhoneNumber = userDTO.PhoneNumber;

            var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);
            var addedRoles = userDTO.Roles.Except(userRoles);
            var removedRoles = userRoles.Except(userDTO.Roles);

            await _unitOfWork.UserManager.AddToRolesAsync(user, addedRoles);

            await _unitOfWork.UserManager.RemoveFromRolesAsync(user, removedRoles);

            await _unitOfWork.UserManager.UpdateAsync(user);
            //_unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}








