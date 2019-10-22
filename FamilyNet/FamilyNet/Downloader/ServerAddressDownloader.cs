using DataTransferObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class ServerAddressDownloader : IServerAddressDownloader
    {
        public async Task<IEnumerable<AddressDTO>> GetAllAsync(string url)
        {
            List<AddressDTO> objs = null;

            try
            {
                HttpResponseMessage response = null;

                using (var httpClient = new HttpClient())
                {
                    response = await httpClient.GetAsync(url);
                }

                var json = await response.Content.ReadAsStringAsync();
                objs = JsonConvert.DeserializeObject<List<AddressDTO>>(json);
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
        public async Task<HttpResponseMessage> CreatePostAsync(string url,
                                                   AddressDTO dto)
        {
            HttpResponseMessage msg = null;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFprmData(dto, formDataContent);

                msg = await httpClient.PostAsync(url, formDataContent);
            }

            return msg;
        }

        public async Task<HttpResponseMessage> CreatePutAsync(string url,
                                                              AddressDTO dto)
        {
            HttpResponseMessage msg = null;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFprmData(dto, formDataContent);

                msg = await httpClient.PutAsync(url, formDataContent);

            }

            return msg;
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


        public async Task<AddressDTO> GetByIdAsync(string url)
        {
            AddressDTO obj = null;

            try
            {
                HttpResponseMessage response = null;

                using (var httpClient = new HttpClient())
                {
                    response = await httpClient.GetAsync(url);
                }

                var json = await response.Content.ReadAsStringAsync();
                obj = JsonConvert.DeserializeObject<AddressDTO>(json);
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

        public async Task<HttpStatusCode> DeleteAsync(string url)
        {
            HttpResponseMessage response;

            try
            {
                using (var httpClient = new HttpClient())
                {
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
    }
}
