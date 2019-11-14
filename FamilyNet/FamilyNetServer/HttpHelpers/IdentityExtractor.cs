using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace FamilyNetServer.HttpHandlers
{
    public class IdentityExtractor : IIdentityExtractor
    {
        public string GetId(ClaimsPrincipal user)
        {
            return user.Identity.Name;
        }

        public string GetSignature(HttpContext httpContext)
        {
            var headerAuthorize = "Authorization";
            var header = httpContext.Request.Headers[headerAuthorize];
            var signature = String.Empty;

            if (!String.IsNullOrEmpty(header))
            {
                var parts = header.ToString().Split(".");
                int countItemsOfJWT = 3;

                if (parts.Length >= countItemsOfJWT)
                {
                    signature = parts[countItemsOfJWT - 1];
                }
            }

            return signature;
        }
    }
}
