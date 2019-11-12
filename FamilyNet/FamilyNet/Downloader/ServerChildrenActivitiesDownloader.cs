using DataTransferObjects;
using FamilyNet.HttpHandlers;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class ServerChildrenActivitiesDownloader : ServerSimpleDataDownloader<ChildActivityDTO>
    {
        public ServerChildrenActivitiesDownloader(IHttpAuthorizationHandler authorizationHandler)
            : base(authorizationHandler) { }

        public override async Task<HttpResponseMessage> CreatePostAsync(string url,
                                                            ChildActivityDTO dto,
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



        public override async Task<HttpResponseMessage> CreatePutAsync(string url,
                                                              ChildActivityDTO dto,
                                                              ISession session)
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


        private void BuildMultipartFormData(ChildActivityDTO dto,
                                            MultipartFormDataContent formDataContent)
        {
            if (dto.ID > 0)
            {
                formDataContent.Add(new StringContent(dto.ID.ToString()), "ID");
            }

            formDataContent.Add(new StringContent(dto.Name), "Name");
            formDataContent.Add(new StringContent(dto.Description), "Description");
            formDataContent.Add(new StringContent(dto.ChildID.ToString()), "ChildID");

            int counter = 0;

            if (dto.Awards != null)
            {
                foreach (var a in dto.Awards)
                {
                    if (a.ID > 0)
                    {
                        formDataContent.Add(new StringContent(a.ID.ToString()), "Awards[" + counter + "].ID");
                    }

                    formDataContent.Add(new StringContent(a.Name), "Awards[" + counter + "].Name");
                    formDataContent.Add(new StringContent(a.Description), "Awards[" + counter + "].Description");
                    formDataContent.Add(new StringContent(a.Date.ToString()), "Awards[" + counter + "].Date");

                    counter++;

                }
            }
        }
    }
}