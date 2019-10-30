using DataTransferObjects;
using FamilyNet.HttpHandlers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;


namespace FamilyNet.Downloader
{
    public class ServerRoleDownloader : ServerSimpleDataDownloader<RoleDTO>
    {
        public ServerRoleDownloader(IHttpAuthorizationHandler authorizationHandler)
           : base(authorizationHandler) { }

        public override async Task<HttpResponseMessage> CreatePostAsync(string url,
                                                               RoleDTO dto, ISession session)
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
                                                                  RoleDTO dto, ISession session)
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

        private static void BuildMultipartFormData(RoleDTO dto,
                                                   MultipartFormDataContent formDataContent)
        {
            if (dto.Name != null)
            {
                formDataContent.Add(new StringContent(dto.Name.ToString()), "Name");
            }

        }
    }
}
