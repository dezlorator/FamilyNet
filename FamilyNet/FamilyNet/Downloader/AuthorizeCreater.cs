using DataTransferObjects;
using FamilyNet.Configuration;
using FamilyNet.HttpHandlers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class AuthorizeCreater : IAuthorizeCreater
    {
        private readonly ServerURLSettings _serverURL;

        public AuthorizeCreater(IOptionsSnapshot<ServerURLSettings> options)
        {
            _serverURL = options.Value;
        }

        public async Task<AuthenticationResult> Login(CredentialsDTO credentials)
        {
            var authenticationResult = new AuthenticationResult();
            var url = _serverURL.ServerURL + "api/v1/authentication";

            using (var httpClient = new HttpClient())
            {               
                var content = new StringContent(JsonConvert.SerializeObject(credentials),
                                                Encoding.UTF8, "application/json");

                var result = await httpClient.PostAsync(url, content);
                var json = await result.Content.ReadAsStringAsync();

                try
                {
                    var token = JsonConvert.DeserializeObject<TokenDTO>(json);
                    authenticationResult.Token = token;
                    authenticationResult.Success = true;
                }
                catch (JsonException)
                {
                    authenticationResult.Token = null;
                    authenticationResult.Success = false;
                }
            }

            return authenticationResult;
        }
    }
}
