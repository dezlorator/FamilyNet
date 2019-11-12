using DataTransferObjects;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using FamilyNetServer.HttpHandlers;
using Newtonsoft.Json;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        #region private fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RolesController> _logger;
        private readonly IIdentityExtractor _identityExtractor;

        #endregion

        #region ctor

        public RolesController(IUnitOfWork unitOfWork,
                               ILogger<RolesController> logger,
                               IIdentityExtractor identityExtractor)
        {
            _unitOfWork = unitOfWork;
            _identityExtractor = identityExtractor;
            _logger = logger;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetAllAsync()
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Roles/api/v1 [GET] was called", userId, token);

            var allRoles = _unitOfWork.RoleManager.Roles.ToList();

            _logger.LogInformation("{status},{json}",
                StatusCodes.Status200OK,
                JsonConvert.SerializeObject(allRoles));

            return Ok(allRoles);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync([FromForm]RoleDTO roleDTO)
        {
            var userId = _identityExtractor.GetId(User);
            var token = _identityExtractor.GetSignature(HttpContext);

            _logger.LogInformation("{info}{userId}{token}",
                "Endpoint Roles/api/v1 [POST] was called", userId, token);

            if (roleDTO != null)
            {
                await _unitOfWork.RoleManager.CreateAsync(new IdentityRole()
                {
                    Name = roleDTO.Name,
                });

                _unitOfWork.SaveChanges();

                _logger.LogInformation("{token}{userId}{status}{info}",
                    token, userId, StatusCodes.Status201Created,
                    $"Role was saved");

                return Created("api/v1/roles/", roleDTO);
            }

            _logger.LogWarning("{status}{token}{userId}{info}",
                StatusCodes.Status400BadRequest, token, userId,
                "RoleDTO is null");

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
                "Endpoint Roles/api/v1 [DELETE] was called", userId, token);

            if (id != null)
            {
                var role = await _unitOfWork.RoleManager.FindByIdAsync(id);

                if (role != null)
                {
                    await _unitOfWork.RoleManager.DeleteAsync(role);
                }

                _unitOfWork.SaveChanges();

                _logger.LogInformation("{status} {info} {userId} {token}",
                    StatusCodes.Status200OK, $"Role was deleted",
                    userId, token);

                return Ok();
            }

            _logger.LogWarning("{status}{token}{userId}{info}",
                StatusCodes.Status400BadRequest, token, userId,
                "Role was not found");

            return BadRequest();
        }
    }
}
