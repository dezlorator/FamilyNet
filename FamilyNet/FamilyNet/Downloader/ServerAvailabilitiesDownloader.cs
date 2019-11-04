using DataTransferObjects;
using FamilyNet.HttpHandlers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class ServerAvailabilitiesDownloader : IServerAvailabilitiesDownloader
    {
        private readonly IHttpAuthorizationHandler _authorizationHandler;

        public ServerAvailabilitiesDownloader(IHttpAuthorizationHandler authorizationHandler)
        {
            _authorizationHandler = authorizationHandler;
        }

        public async Task<IEnumerable<AvailabilityDTO>> GetAllAsync(string url,
                                                     ISession session)
        {
            List<AvailabilityDTO> objs = null;

            try
            {
                HttpResponseMessage response = null;

                using (var httpClient = new HttpClient())
                {
                    _authorizationHandler.AddTokenBearer(session, httpClient);
                    response = await httpClient.GetAsync(url);
                }

                var json = await response.Content.ReadAsStringAsync();
                objs = JsonConvert.DeserializeObject<List<AvailabilityDTO>>(json);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (JsonException)
            {
                throw;
            }

            return objs;
        }

        public async Task<HttpStatusCode> CreatePostAsync(string url,
                                                     AvailabilityDTO dto, ISession session)
        {
            var statusCode = HttpStatusCode.BadRequest;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                _authorizationHandler.AddTokenBearer(session, httpClient);
                BuildMultipartFprmData(dto, formDataContent);
                var msg = await httpClient.PostAsync(url, formDataContent);
                statusCode = msg.StatusCode;
            }

            return statusCode;
        }

        private static void BuildMultipartFprmData(AvailabilityDTO dto,
                                                   MultipartFormDataContent formDataContent)
        {

            if (dto.ID > 0)
            {
                formDataContent.Add(new StringContent(dto.ID.ToString()), "ID");
            }

            //formDataContent.Add(new StringContent(dto.Date.ToString()), "Date");
            formDataContent.Add(new StringContent(dto.DayOfWeek.ToString()), "DayOfWeek");
            formDataContent.Add(new StringContent(dto.FromHour.ToString()), "FromHour");
            formDataContent.Add(new StringContent(dto.VolunteerHours.ToString()), "VolunteerHours");
        }
    }
}
