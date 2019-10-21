using FamilyNet.HttpHandlers;
using Microsoft.AspNetCore.Http;
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
        #region private fields

        protected readonly IHttpAuthorizationHandler _authorizationHandler;

        #endregion

        #region ctor

        public ServerDataDownLoader(IHttpAuthorizationHandler authorizationHandler)
        {
            _authorizationHandler = authorizationHandler;
        }

        #endregion

        public async Task<IEnumerable<T>> GetAllAsync(string url,
                                                      ISession session)
        {
            List<T> objs = null;

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
            T obj = null;

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

        public abstract Task<HttpStatusCode> СreatetePostAsync(string url,
                                                               T dto,
                                                               Stream file,
                                                               string fieName,
                                                               ISession session);

        public abstract Task<HttpStatusCode> СreatetePutAsync(string url,
                                                               T dto,
                                                               Stream file,
                                                               string fieName,
                                                               ISession session);

        public async Task<HttpStatusCode> DeleteAsync(string url,
                                                      ISession session)
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
    }
}
