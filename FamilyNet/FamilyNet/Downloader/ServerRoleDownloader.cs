using DataTransferObjects;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace FamilyNet.Downloader
{
    public class ServerRoleDownloader : ServerSimpleDataDownloader<RoleDTO>
    {
        public override async Task<HttpResponseMessage> CreatePostAsync(string url,
                                                               RoleDTO dto)
        {
            HttpResponseMessage msg = null;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFormData(dto, formDataContent);

                msg = await httpClient.PostAsync(url, formDataContent);
            }

            return msg;
        }

        public override async Task<HttpResponseMessage> CreatePutAsync(string url,
                                                                  RoleDTO dto)
        {
            HttpResponseMessage msg = null;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFormData(dto, formDataContent);

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
