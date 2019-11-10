using Microsoft.AspNetCore.Http;

namespace FamilyNetServer.HttpHandlers
{
    public interface ITokenSignatureExtractor
    {
        string GetSignature(HttpContext httpContext);
    }
}
