using System.Threading.Tasks;
using DataTransferObjects;
using FamilyNetServer.Factories;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FamilyNetServer.Controllers.API.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        #region private fields

        private readonly ITokenFactory _tokenFactory;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region ctor

        public AuthenticationController(IUnitOfWork unitOfWork,
                                        ITokenFactory tokenFactory,
                                        ILogger<AuthenticationController> logger)
        {
            _tokenFactory = tokenFactory;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #endregion

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> Authentication([FromBody]CredentialsDTO credentialsDTO)
        {
            _logger.LogInformation("Authentication method is called. Arguments password: " +
                credentialsDTO.Password + " email: " + credentialsDTO.Email);
            var user = await _unitOfWork.UserManager.FindByEmailAsync(credentialsDTO.Email);

            if (user == null)
            {
                var msg = "Credentials are invalid!";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            if (!await _unitOfWork.UserManager.IsEmailConfirmedAsync(user))
            {
                var msg = "User's email was not confirmed!";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var result = await _unitOfWork.UserManager.CheckPasswordAsync(user, credentialsDTO.Password);

            if (!result)
            {
                var msg = "Credentials are invalid!";
                _logger.LogError(msg);
                return BadRequest(msg);
            }

            var roles = await _unitOfWork.UserManager.GetRolesAsync(user).ConfigureAwait(false);
            var token = _tokenFactory.Create(user, roles);
            _logger.LogInformation("User " + credentialsDTO.Email + " has token " +
                token.Token);

            return Created("", token);
        }

        [HttpGet]
        public void SeedData()
        {
            var seedData = new SeedData(_unitOfWork);
            seedData.EnsurePopulated();
        }
    }
}