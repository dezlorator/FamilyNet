using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;


namespace FamilyNet.HttpHandlers
{
    public class HttpAuthorizationHandler : IHttpAuthorizationHandler
    {        
        public void AddTokenBearer(HttpRequest Request, HttpClient httpClient)
        {
            if (Request.Headers.Keys.Contains("Bearer"))
            {
                foreach (var header in Request.Headers)
                {
                    if (header.Key == "Bearer")
                    {
                        httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue(header.Key, header.Value);
                        break;
                    }
                }
            }
        }
    }
}
