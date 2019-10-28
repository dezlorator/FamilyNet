using DataTransferObjects;
using FamilyNet.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

        public async Task<string> Login(CredentialsDTO credentials)
        {
            string token = String.Empty;
            var url = _serverURL.ServerURL + "api/v1/authentication";

            using (var httpClient = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("Email", credentials.Email),
                        new KeyValuePair<string, string>("Password", credentials.Email)
                    });

                var result = await httpClient.PostAsync(url, content);
                var json = await result.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<TokenDTO>(json).Token;
            }

            return token;
        }
    }
}
