using System.Linq;
using FamilyNetServer.Configuration;
using FamilyNetServer.Models.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FamilyNetServer.Factories
{
    public class TokenFactory : ITokenFactory
    {
        #region private fields

        private JWTConfiguration _JWTConfiguration { get; set; }

        #endregion

        #region ctor

        public TokenFactory(IOptions<JWTConfiguration> JWTConfiguration)
        {
            _JWTConfiguration = JWTConfiguration.Value;
        }

        #endregion

        public string Create(ApplicationUser user, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_JWTConfiguration.Secret);

            var cliams = new List<Claim>();
            cliams.Add(new Claim(ClaimTypes.Name, user.Id));
            roles.ToList().ForEach(r => cliams.Add(new Claim(ClaimTypes.Role, r)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(cliams),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                         SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
