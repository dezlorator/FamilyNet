using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class AuthorizeCreater : IAuthorizeCreater
    {
        public async Task<string> Login(string email, string password)
        {
            var token = String.Empty;
            var url = String.Empty;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                formDataContent.Add(new StringContent(email), "email");
                formDataContent.Add(new StringContent(password), "password");
                var msg = await httpClient.PostAsync(url, formDataContent);
            }

            return token;
        }
    }
}
