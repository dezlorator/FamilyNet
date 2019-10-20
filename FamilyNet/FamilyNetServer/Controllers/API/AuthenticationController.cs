using System.Threading.Tasks;
using System.Web.Http;
using FamilyNetServer.DTO;
using FamilyNetServer.Factories;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
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

        [System.Web.Http.HttpPost]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<TokenDTO> Authentication([FromBody]CredentialsDTO credentialsDTO)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(credentialsDTO.Email);
            var roles = await _unitOfWork.UserManager.GetRolesAsync(user).ConfigureAwait(false);

            return new TokenDTO() { Token = _tokenFactory.Create(user, roles) };
        }
    }
}