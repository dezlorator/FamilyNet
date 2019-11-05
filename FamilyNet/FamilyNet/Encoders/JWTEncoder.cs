using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace FamilyNet.Encoders
{
    public class JWTEncoder : IJWTEncoder
    {
        public TokenClaims GetTokenData(string token)
        {
            var tokenClaims = new TokenClaims();
            var jwt = new JwtSecurityToken(token);
            var claims = jwt.Claims.ToList();

            tokenClaims.Roles = claims
                .Where(c => c.Type == "role")
                .Select(c => c.Value)
                .ToList();

            var id = claims?.FirstOrDefault(c => c.Type == "unique_name")?.Value;

            if (!String.IsNullOrEmpty(id))
            {
                tokenClaims.UserId = new Guid(id);
            }

            var personId = claims?.FirstOrDefault(c => c.Type == "nameid")?.Value;

            if (!String.IsNullOrEmpty(personId))
            {
                if (int.TryParse(personId, out int person))
                {
                    tokenClaims.PersonId = person;
                }
            }

            tokenClaims.Email = jwt.Claims?.FirstOrDefault(c => c.Type == "email").Value;

            return tokenClaims;
        }
    }
}
