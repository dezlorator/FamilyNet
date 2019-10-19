using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace FamilyNet.HttpHandlers
{
    public interface IHttpAuthorizationHandler
    {
        void AddTokenBearer(HttpRequest Request, HttpClient httpClient);
    }
}
