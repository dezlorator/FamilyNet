using DataTransferObjects;
using FamilyNet.HttpHandlers;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class ServerChildrenHouseDownloader : ServerDataDownloader<ChildrenHouseDTO>
    {

        public ServerChildrenHouseDownloader(IHttpAuthorizationHandler authorizationHandler)
           : base(authorizationHandler)
        { }

        public override async Task<HttpStatusCode> CreatePostAsync(string url,
                                                              ChildrenHouseDTO dto,
                                                              Stream streamFile,
                                                              string fieName,
                                                              ISession session)
        {
            var statusCode = HttpStatusCode.BadRequest;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFprmData(dto, streamFile, formDataContent);
                _authorizationHandler.AddTokenBearer(session, httpClient);
                var msg = await httpClient.PostAsync(url, formDataContent);
                statusCode = msg.StatusCode;

                if (streamFile != null)
                {
                    streamFile.Close();
                }
            }

            return statusCode;
        }

        public override async Task<HttpStatusCode> CreatePutAsync(string url,
                                                              ChildrenHouseDTO dto,
                                                              Stream streamFile,
                                                              string fileName,
                                                              ISession session)
        {
            var statusCode = HttpStatusCode.BadRequest;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFprmData(dto, streamFile, formDataContent);
                _authorizationHandler.AddTokenBearer(session, httpClient);
                var msg = await httpClient.PutAsync(url, formDataContent);
                statusCode = msg.StatusCode;

                if (streamFile != null)
                {
                    streamFile.Close();
                }
            }

            return statusCode;
        }

        private static void BuildMultipartFprmData(ChildrenHouseDTO dto,
                                                   Stream streamFile,
                                                   MultipartFormDataContent formDataContent)
        {
            if (streamFile != null && streamFile.Length > 0)
            {
                var image = new StreamContent(streamFile, (int)streamFile.Length);
                formDataContent.Add(image, "Avatar", dto.Avatar.FileName);
            }

            if (dto.ID > 0)
            {
                formDataContent.Add(new StringContent(dto.ID.ToString()), "ID");
            }

            formDataContent.Add(new StringContent(dto.Name), "Name");
            formDataContent.Add(new StringContent(dto.Rating.ToString()), "Rating");
            //formDataContent.Add(new StringContent(dto.PhotoPath), "PhotoPath");
            formDataContent.Add(new StringContent(dto.LocationID.ToString()), "LocationID");
            formDataContent.Add(new StringContent(dto.AdressID.ToString()), "AdressID");
        }

    }
}
