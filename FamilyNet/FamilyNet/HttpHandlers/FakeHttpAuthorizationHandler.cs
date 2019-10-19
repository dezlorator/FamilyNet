using FamilyNet.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;


namespace FamilyNet.HttpHandlers
{
    public class FakeHttpAuthorizationHandler : IHttpAuthorizationHandler
    {
        private readonly string _token;

        public FakeHttpAuthorizationHandler(IOptionsSnapshot<JWTCofiguration> options)
        {
            _token = options.Value.TestToken;
        }

        public void AddTokenBearer(HttpRequest Request, HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", _token);
        }
    }
}
