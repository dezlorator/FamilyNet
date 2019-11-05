using DataTransferObjects;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _unitOfWork;

        #endregion

        public RolesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAllAsync()
        {
            var allRoles = _unitOfWork.RoleManager.Roles.ToList();
           
            return Ok(allRoles);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromForm]RoleDTO roleDTO)
        {
            if (roleDTO != null)
            {
                await _unitOfWork.RoleManager.CreateAsync(new IdentityRole()
                {
                    Name = roleDTO.Name,
                });

                _unitOfWork.SaveChangesAsync();
                return Created("api/v1/roles/", roleDTO);
                
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            if (id != null)
            {
                IdentityRole role = await _unitOfWork.RoleManager.FindByIdAsync(id);
                if (role != null)
                {
                    await _unitOfWork.RoleManager.DeleteAsync(role);
                }

                _unitOfWork.SaveChangesAsync();


                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }

}
