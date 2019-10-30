using System.Threading.Tasks;
using DataTransferObjects;
using FamilyNetServer.Factories;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : BaseController
    {
        #region private fields

        private readonly ITokenFactory _tokenFactory;
        private readonly ILogger<AuthenticationController> _logger;

        #endregion

        #region ctor

        public AuthenticationController(IUnitOfWork unitOfWork,
                                        ITokenFactory tokenFactory,
                                        ILogger<AuthenticationController> logger)
            : base(unitOfWork)
        {
            _tokenFactory = tokenFactory;
            _logger = logger;
        }

        #endregion

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> Authentication([FromForm]CredentialsDTO credentialsDTO)
        {
            _logger.LogInformation("Authentication method is called. Arguments password: " +
                credentialsDTO.Password + " email: " + credentialsDTO.Email);
            var user = await _unitOfWork.UserManager.FindByEmailAsync(credentialsDTO.Email);

            if (user == null)
            {
                _logger.LogError("User was not found!");
                return BadRequest();
            }

            var roles = await _unitOfWork.UserManager.GetRolesAsync(user).ConfigureAwait(false);
            var token = new TokenDTO() { Token = _tokenFactory.Create(user, roles) };
            _logger.LogInformation("User " + credentialsDTO.Email + " has token " +
                token.Token);

            return Created("", token);
        }
    }
}