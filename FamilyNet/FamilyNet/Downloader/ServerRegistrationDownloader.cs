using DataTransferObjects;
using FamilyNet.HttpHandlers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;


namespace FamilyNet.Downloader
{
    public class ServerRegistrationDownloader : ServerSimpleDataDownloader<UserDTO>
    {
        public ServerRegistrationDownloader(IHttpAuthorizationHandler authorizationHandler)
        :base(authorizationHandler){}

        public override async Task<HttpResponseMessage> CreatePostAsync(string url,
                                                               UserDTO dto, ISession session)
        {
            HttpResponseMessage msg = null;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFormData(dto, formDataContent);
                _authorizationHandler.AddTokenBearer(session, httpClient);
                msg = await httpClient.PostAsync(url, formDataContent);
            }

            return msg;
        }

        public override async Task<HttpResponseMessage> CreatePutAsync(string url,
                                                                  UserDTO dto, ISession session)
        {
            HttpResponseMessage msg = null;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFormData(dto, formDataContent);
                _authorizationHandler.AddTokenBearer(session, httpClient);
                msg = await httpClient.PutAsync(url, formDataContent);
            }

            return msg;
        }

        private static void BuildMultipartFormData(UserDTO dto,
                                                   MultipartFormDataContent formDataContent)
        {
            if (dto.Email != null)
            {
                formDataContent.Add(new StringContent(dto.Email.ToString()), "Email");
            }
            if (dto.PhoneNumber != null)
            {
                formDataContent.Add(new StringContent(dto.PhoneNumber.ToString()), "PhoneNumber");
            }
            if (dto.Password != null)
            {
                formDataContent.Add(new StringContent(dto.Password.ToString()), "Password");
            }
            if (dto.Roles != null)
            {
                formDataContent.Add(new StringContent(dto.Roles.ToString()), "Role");
            }


        }
    }
}
