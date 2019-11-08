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

            tokenClaims.Roles = jwt.Claims.ToList()
                .Where(c => c.Type == "role")
                .Select(c => c.Value)
                .ToList();

            var id = jwt.Claims?.ToList()?.FirstOrDefault(c => c.Type == "unique_name")?.Value;

            if (!String.IsNullOrEmpty(id))
            {
                tokenClaims.UserId = new Guid(id);
            }

            tokenClaims.Email = jwt.Claims?.FirstOrDefault(c => c.Type == "email").Value;

            return tokenClaims;
        }
    }
}
