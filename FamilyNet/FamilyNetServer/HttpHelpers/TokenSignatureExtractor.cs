using Microsoft.AspNetCore.Http;
using System;

namespace FamilyNetServer.HttpHandlers
{
    public class TokenSignatureExtractor : ITokenSignatureExtractor
    {
        public string GetSignature(HttpContext httpContext)
        {
            var header = httpContext.Request.Headers["Authorization"];

            if (!String.IsNullOrEmpty(header))
            {
                var parts = header.ToString().Split(".");
                int countItemsOfJWT = 3;

                if (parts.Length >= countItemsOfJWT)
                {
                    return parts[countItemsOfJWT - 1];
                }
            }

            return null;
        }
    }
}
