using DataTransferObjects;
using FamilyNet.HttpHandlers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class ServerQuestsDownloader : ServerSimpleDataDownloader<QuestDTO>
    {
        public ServerQuestsDownloader(IHttpAuthorizationHandler authorizationHandler)
               : base(authorizationHandler) { }

        public override async Task<HttpResponseMessage> CreatePostAsync(string url, QuestDTO dto, ISession session)
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

        public override async Task<HttpResponseMessage> CreatePutAsync(string url, QuestDTO dto, ISession session)
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

        private static void BuildMultipartFormData(QuestDTO dto,
                                                  MultipartFormDataContent formDataContent)
        {
            if (dto.ID > 0)
            {
                formDataContent.Add(new StringContent(dto.ID.ToString()), "ID");
            }

            if (dto.DonationID != null)
            {
                formDataContent.Add(new StringContent(dto.DonationID.ToString()), "DonationID");
            }

            if (!string.IsNullOrEmpty(dto.Name))
            {
                formDataContent.Add(new StringContent(dto.Name), "Name");
            }

            if (dto.VolunteerID != null)
            {
                formDataContent.Add(new StringContent(dto.VolunteerID.ToString()), "VolunteerID");
            }

            if (dto.Description != null)
            {
                formDataContent.Add(new StringContent(dto.Description), "Description");
            }

            if (dto.FromDate != null)
            {
                formDataContent.Add(new StringContent(dto.FromDate.ToString()), "FromDate");
            }

            if (dto.ToDate != null)
            {
                formDataContent.Add(new StringContent(dto.ToDate.ToString()), "ToDate");
            }
        }
    }
}