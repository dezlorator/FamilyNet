using DataTransferObjects;
using FamilyNet.HttpHandlers;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class ServerFeedbackDownloader : ServerDataDownloader<FeedbackDTO>
    {
        public ServerFeedbackDownloader(IHttpAuthorizationHandler authorizationHandler)
        : base(authorizationHandler)
        { }
        public override async Task<HttpStatusCode> CreatePostAsync(string url, FeedbackDTO dto, Stream file, string fileName, ISession session)
        {
            var statusCode = HttpStatusCode.BadRequest;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFormData(dto, file, fileName, formDataContent);
                _authorizationHandler.AddTokenBearer(session, httpClient);
                var msg = await httpClient.PostAsync(url, formDataContent);
                statusCode = msg.StatusCode;

                if (file != null)
                {
                    file.Close();
                }
            }

            return statusCode;
        }

        public override async Task<HttpStatusCode> CreatePutAsync(string url, FeedbackDTO dto, Stream file, string fileName, ISession session)
        {
            var statusCode = HttpStatusCode.BadRequest;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFormData(dto, file, fileName, formDataContent);
                _authorizationHandler.AddTokenBearer(session, httpClient);
                var msg = await httpClient.PutAsync(url, formDataContent);
                statusCode = msg.StatusCode;

                if (file != null)
                {
                    file.Close();
                }
            }

            return statusCode;
        }

        private static void BuildMultipartFormData(FeedbackDTO dto,
                                   Stream streamFile,
                                   string fileName,
                                   MultipartFormDataContent formDataContent)
        {
            if (streamFile != null && streamFile.Length > 0)
            {
                var image = new StreamContent(streamFile, (int)streamFile.Length);
                formDataContent.Add(image, "Image", fileName);
            }

            if (dto.ID > 0)
            {
                formDataContent.Add(new StringContent(dto.ID.ToString()), "ID");
            }

            formDataContent.Add(new StringContent(dto.DonationId.ToString()), "DonationId");
            formDataContent.Add(new StringContent(dto.Message.ToString()), "Message");
            formDataContent.Add(new StringContent(dto.ReceiverId.ToString()), "ReceiverId");
            formDataContent.Add(new StringContent(dto.ReceiverRole.ToString()), "ReceiverRole");
            formDataContent.Add(new StringContent(dto.Time.ToString()), "Time");
            formDataContent.Add(new StringContent(dto.Rating.ToString()), "Rating");
        }
    }
}
