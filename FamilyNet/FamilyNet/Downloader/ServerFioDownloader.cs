using DataTransferObjects;
using DataTransferObjects.Enums;
using FamilyNet.Downloader.Interfaces;
using FamilyNet.HttpHandlers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class ServerFioDownloader : IFioDownloader
    {
        #region private fields

        protected readonly IHttpAuthorizationHandler _authorizationHandler;

        #endregion

        #region ctor

        public ServerFioDownloader(IHttpAuthorizationHandler authorizationHandler)
        {
            _authorizationHandler = authorizationHandler;
        }
        #endregion
        public async Task<SNPDTO> GetByIdAsync(string url, ISession session)
        {
            SNPDTO obj = null;

            try
            {
                HttpResponseMessage response = null;

                using(var httpClient = new HttpClient())
                using(var formDataContent = new MultipartFormDataContent())
                {
                    _authorizationHandler.AddTokenBearer(session, httpClient);
                    response = await httpClient.GetAsync(url);
                }

                var json = await response.Content.ReadAsStringAsync();
                obj = JsonConvert.DeserializeObject<SNPDTO>(json);
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
    }
}
