using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FamilyNetServer.Configuration;
using FamilyNetServer.DTO;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace FamilyNetServer.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        #region private fields

        private JWTCofiguration _JWTCofiguration { get; set; }

        #endregion

        #region ctor

        public AuthenticationController(IUnitOfWork unitOfWork,
                                        IOptions<JWTCofiguration> JWTConfiguration)
            : base(unitOfWork)
        {
            _JWTCofiguration = JWTConfiguration.Value;
        }

        #endregion

        [System.Web.Http.HttpPost]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<TokenDTO> Authentication([FromBody]CredentialsDTO credentialsDTO)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(credentialsDTO.Email);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_JWTCofiguration.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                         SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenDTO()
            {
                Token = tokenHandler.WriteToken(token)
            };
        }
    }
}