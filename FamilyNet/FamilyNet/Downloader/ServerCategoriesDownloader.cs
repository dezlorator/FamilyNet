using DataTransferObjects;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class ServerCategoriesDownloader : ServerSimpleDataDownloader<CategoryDTO>
    {
        public override async Task<HttpResponseMessage> CreatePostAsync(string url, CategoryDTO dto)
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
        
        public override Task<HttpResponseMessage> CreatePutAsync(string url, CategoryDTO dto)
        {
            throw new System.NotImplementedException();
        }

        private static void BuildMultipartFormData(CategoryDTO dto,
                                                   MultipartFormDataContent formDataContent)
        {
            if (dto.ID > 0)
            {
                formDataContent.Add(new StringContent(dto.ID.ToString()), "ID");
            }

            formDataContent.Add(new StringContent(dto.Name), "Name");
        }
    }
}
