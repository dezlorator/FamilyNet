using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public abstract class ServerDataDownLoader<T> where T : class, new()
    {
        public async Task<IEnumerable<T>> GetAllAsync(string url)
        {
            List<T> objs = null;

            try
            {
                HttpResponseMessage response = null;

                using (var httpClient = new HttpClient())
                {
                    response = await httpClient.GetAsync(url);
                }

                var json = await response.Content.ReadAsStringAsync();
                objs = JsonConvert.DeserializeObject<List<T>>(json);
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

        public async Task<T> GetByIdAsync(string url)
        {
            T obj = null;

            try
            {
                HttpResponseMessage response = null;

                using (var httpClient = new HttpClient())
                {
                    response = await httpClient.GetAsync(url);
                }

                var json = await response.Content.ReadAsStringAsync();
                obj = JsonConvert.DeserializeObject<T>(json);
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

        public abstract Task<HttpStatusCode> СreatePostAsync(string url,
                                                               T dto,
                                                               Stream file,
                                                               string fieName);

        public abstract Task<HttpStatusCode> СreatePutAsync(string url,
                                                               T dto,
                                                               Stream file,
                                                               string fieName);

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
