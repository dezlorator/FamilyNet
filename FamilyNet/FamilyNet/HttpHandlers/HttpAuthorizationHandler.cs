using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;


namespace FamilyNet.HttpHandlers
{
    public class HttpAuthorizationHandler : IHttpAuthorizationHandler
    {        
        public void AddTokenBearer(ISession session, HttpClient httpClient)
        {
            var token = session.GetString("Bearer");

            httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
