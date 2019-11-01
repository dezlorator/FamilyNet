using FamilyNetServer.DTO;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly ILogger<RolesController> _logger;

        #endregion

        public RolesController(IUnitOfWorkAsync unitOfWork, ILogger<RolesController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAllAsync()
        {
            var allRoles = _unitOfWork.RoleManager.Roles.ToList();
            _logger.LogInformation("0k[200]. List of roles was sent.");
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
                _logger.LogInformation("Return Created[201].New role was added.");
                return Created("api/v1/roles/", roleDTO);
                
            }
            else
            {
                _logger.LogError("Bad request[400]. RoleDTO is not valid. Role was not created.");
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
                _logger.LogInformation("Return Ok[200].Role was deleted.");

                return Ok();
            }
            else
            {
                _logger.LogError("Bad request[400]. Id is not valid. Role was not deleted.");
                return BadRequest();
            }
        }
    }

}
