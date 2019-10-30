using System.Threading.Tasks;
using DataTransferObjects;
using FamilyNetServer.Factories;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : BaseController
    {
        #region private fields

        private readonly ITokenFactory _tokenFactory;

        #endregion

        #region ctor

        public AuthenticationController(IUnitOfWork unitOfWork,
                                        ITokenFactory tokenFactory)
            : base(unitOfWork)
        {
            _tokenFactory = tokenFactory;
        }

        #endregion

        [HttpPost]
        [Produces("application/json")]        
        public async Task<IActionResult> Authentication([FromForm]CredentialsDTO credentialsDTO)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(credentialsDTO.Email);

            if (user == null)
            {
                return BadRequest();
            }

            var roles = await _unitOfWork.UserManager.GetRolesAsync(user).ConfigureAwait(false);

            return Created("", new TokenDTO() { Token = _tokenFactory.Create(user, roles) });
        }
    }
}