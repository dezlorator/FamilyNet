using FamilyNet.HttpHandlers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public abstract class ServerSimpleDataDownloader<T> where T : class, new()
    {
        protected readonly IHttpAuthorizationHandler _authorizationHandler;

        public int TotalItemsCount { get; protected set; }

        public ServerSimpleDataDownloader(IHttpAuthorizationHandler authorizationHandler)
        {
            _authorizationHandler = authorizationHandler;
        }

        public async virtual Task<IEnumerable<T>> GetAllAsync(string url, ISession session)
        {
            List<T> objs;

            try
            {
                HttpResponseMessage response = null;

                using (var httpClient = new HttpClient())
                {
                    _authorizationHandler.AddTokenBearer(session, httpClient);
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

        public async Task<T> GetByIdAsync(string url, ISession session)
        {
            T obj;

            try
            {
                HttpResponseMessage response = null;

                using (var httpClient = new HttpClient())
                {
                    _authorizationHandler.AddTokenBearer(session, httpClient);
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

        public abstract Task<HttpResponseMessage> CreatePostAsync(string url,
                                                               T dto, ISession session);

        public abstract Task<HttpResponseMessage> CreatePutAsync(string url,
                                                               T dto, ISession session);

        public async Task<HttpResponseMessage> DeleteAsync(string url, ISession session)
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

            return response;
        }
    }
}
