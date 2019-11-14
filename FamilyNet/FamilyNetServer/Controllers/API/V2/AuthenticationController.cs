using System.Threading.Tasks;
using DataTransferObjects;
using FamilyNetServer.Factories;
using FamilyNetServer.Models;
using FamilyNetServer.Models.EntityFramework;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FamilyNetServer.Controllers.API.V2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        #region private fields

        private readonly ITokenFactory _tokenFactory;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly EFRepository<ChildActivity> _childActivityRepository;

        #endregion

        #region ctor

        public AuthenticationController(IUnitOfWork unitOfWork,
                                        EFRepository<ChildActivity> childActivityRepository,
                                        ITokenFactory tokenFactory,
                                        ILogger<AuthenticationController> logger)
        {
            _tokenFactory = tokenFactory;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _childActivityRepository = childActivityRepository;
        }

        #endregion

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> Authentication([FromBody]CredentialsDTO credentialsDTO)
        {
            _logger.LogInformation("Endpoint Authentication/api/v2 " +
                " [POST] was called. Password: " + credentialsDTO.Password +
                " email: " + credentialsDTO.Email);

            var user = await _unitOfWork.UserManager
                .FindByEmailAsync(credentialsDTO.Email);

            if (user == null)
            {
                var msg = "Credentials are invalid!";
                _logger.LogError("{info}{status}", msg,
                    StatusCodes.Status400BadRequest);

                return BadRequest(msg);
            }

            if (!await _unitOfWork.UserManager.IsEmailConfirmedAsync(user))
            {
                var msg = "User's email was not confirmed!";
                _logger.LogError("{info}{status}", msg,
                    StatusCodes.Status400BadRequest);

                return BadRequest(msg);
            }

            var result = await _unitOfWork.UserManager.CheckPasswordAsync(user,
                             credentialsDTO.Password);

            if (!result)
            {
                var msg = "Credentials are invalid!";
                _logger.LogError("{info}{status}", msg,
                    StatusCodes.Status400BadRequest);

                return BadRequest(msg);
            }

            var roles = await _unitOfWork.UserManager.GetRolesAsync(user)
                .ConfigureAwait(false);

            var token = _tokenFactory.Create(user, roles);

            _logger.LogInformation("{info}{status}{token}", "Token was created ",
                StatusCodes.Status201Created, token.Token);

            return Created("", token);
        }

        [HttpGet]
        public void SeedData()
        {
            var seedData = new SeedData(_unitOfWork, _childActivityRepository);
            seedData.EnsurePopulated();
        }
    }
}