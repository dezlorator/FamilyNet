using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FamilyNetServer.HttpHandlers
{
    public interface IIdentityExtractor
    {
        string GetSignature(HttpContext httpContext);
        string GetId(ClaimsPrincipal user);
    }
}
