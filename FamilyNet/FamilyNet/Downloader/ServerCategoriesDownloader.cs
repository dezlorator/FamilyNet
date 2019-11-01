using DataTransferObjects;
using FamilyNet.HttpHandlers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class ServerCategoriesDownloader : ServerSimpleDataDownloader<CategoryDTO>
    {
        public ServerCategoriesDownloader(IHttpAuthorizationHandler authorizationHandler)
            : base(authorizationHandler){}

        public override async Task<HttpResponseMessage> CreatePostAsync(string url, CategoryDTO dto,
                                                                        ISession session)
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
        
        public override Task<HttpResponseMessage> CreatePutAsync(string url, CategoryDTO dto, ISession session)
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
