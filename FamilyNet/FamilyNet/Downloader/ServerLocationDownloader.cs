using DataTransferObjects;
using FamilyNet.HttpHandlers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class ServerLocationDownloader
    {
        private readonly IHttpAuthorizationHandler _authorizationHandler;

        public ServerLocationDownloader(IHttpAuthorizationHandler authorizationHandler)
        {
            _authorizationHandler = authorizationHandler;
        }

        public async Task<HttpResponseMessage> СreatePostAsync(string url,
                                                           AddressDTO dto, 
                                                           ISession session)
        {
            HttpResponseMessage msg = null;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFprmData(dto, formDataContent);
                _authorizationHandler.AddTokenBearer(session, httpClient);
                msg = await httpClient.PostAsync(url, formDataContent);
            }

            return msg;
        }

        public async Task<HttpResponseMessage> СreatePutAsync(string url,
                                                           AddressDTO dto,
                                                           ISession session)
        {
            HttpResponseMessage msg = null;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFprmData(dto, formDataContent);
                _authorizationHandler.AddTokenBearer(session, httpClient);
                msg = await httpClient.PutAsync(url, formDataContent);
            }

            return msg;
        }

        public async Task<HttpStatusCode> DeleteAsync(string url, ISession session)
        {
            HttpResponseMessage response;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    _authorizationHandler.AddTokenBearer(session, httpClient);
                    response = await httpClient.DeleteAsync(url);
                }
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

            return response.StatusCode;
        }

        public async Task<LocationDTO> GetByIdAsync(string url, ISession session)
        {
            LocationDTO obj = null;

            try
            {
                HttpResponseMessage response = null;

                using (var httpClient = new HttpClient())
                {
                    _authorizationHandler.AddTokenBearer(session, httpClient);
                    response = await httpClient.GetAsync(url);
                }

                var json = await response.Content.ReadAsStringAsync();
                obj = JsonConvert.DeserializeObject<LocationDTO>(json);
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

            return obj;
        }

        private static void BuildMultipartFprmData(AddressDTO dto,
                                                  MultipartFormDataContent formDataContent)
        {
            if (dto.ID > 0)
            {
                formDataContent.Add(new StringContent(dto.ID.ToString()), "ID");
            }

            formDataContent.Add(new StringContent(dto.Country), "Country");
            formDataContent.Add(new StringContent(dto.Region), "Region");
            formDataContent.Add(new StringContent(dto.City), "City");
            formDataContent.Add(new StringContent(dto.Street), "Street");
            formDataContent.Add(new StringContent(dto.House), "House");
        }
    }
}
