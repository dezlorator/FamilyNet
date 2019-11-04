using DataTransferObjects;
using FamilyNet.HttpHandlers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;


namespace FamilyNet.Downloader
{
    public class ServerRegistrationDownloader : ServerSimpleDataDownloader<RegistrationDTO>
    {
        public ServerRegistrationDownloader(IHttpAuthorizationHandler authorizationHandler)
        :base(authorizationHandler){}

        public override async Task<HttpResponseMessage> CreatePostAsync(string url,
                                                               RegistrationDTO dto, ISession session)
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
                                                                  RegistrationDTO dto, ISession session)
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

        private static void BuildMultipartFormData(RegistrationDTO dto,
                                                   MultipartFormDataContent formDataContent)
        {
            if (dto.Email != null)
            {
                formDataContent.Add(new StringContent(dto.Email.ToString()), "Email");
            }
            if (dto.Phone != null)
            {
                formDataContent.Add(new StringContent(dto.Phone.ToString()), "Phone");
            }
            if (dto.Password != null)
            {
                formDataContent.Add(new StringContent(dto.Password.ToString()), "Password");
            }
            if (dto.PasswordConfirm != null)
            {
                formDataContent.Add(new StringContent(dto.PasswordConfirm.ToString()), "PasswordConfirm");
            }
            if (dto.YourDropdownSelectedValue != null)
            {
                formDataContent.Add(new StringContent(dto.YourDropdownSelectedValue.ToString()), "YourDropdownSelectedValue");
            }
        }
    }
}
