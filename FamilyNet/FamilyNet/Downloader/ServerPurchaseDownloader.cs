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
    public class ServerPurchaseDownloader : ServerSimpleDataDownloader<PurchaseDTO>
    {
        public ServerPurchaseDownloader(IHttpAuthorizationHandler authorizationHandler)
            : base(authorizationHandler) { }

        public override async Task<HttpResponseMessage> CreatePostAsync(string url, PurchaseDTO dto,  ISession session)
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

        public override async Task<HttpResponseMessage> CreatePutAsync(string url, PurchaseDTO dto,  ISession session)
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

        public async override Task<IEnumerable<PurchaseDTO>> GetAllAsync(string url,
                                                      ISession session)
        {
            PurchaseFilterDTO objs = null;

            try
            {
                HttpResponseMessage response = null;

                using (var httpClient = new HttpClient())
                {
                    _authorizationHandler.AddTokenBearer(session, httpClient);
                    response = await httpClient.GetAsync(url);
                }

                var json = await response.Content.ReadAsStringAsync();
                objs = JsonConvert.DeserializeObject<PurchaseFilterDTO>(json);
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

            TotalItemsCount = objs.TotalCount;

            return objs.PurchaseDTOs;
        }

        private static void BuildMultipartFormData(PurchaseDTO dto,
                                                  MultipartFormDataContent formDataContent)
        {
            
            if (dto.ID > 0)
            {
                formDataContent.Add(new StringContent(dto.ID.ToString()), "ID");
            }

            formDataContent.Add(new StringContent(dto.UserId), "UserId");
            formDataContent.Add(new StringContent(dto.Quantity.ToString()), "Quantity");
            formDataContent.Add(new StringContent(dto.Paid.ToString()), "Paid");
            formDataContent.Add(new StringContent(dto.Date.ToString()), "Date");
            formDataContent.Add(new StringContent(dto.AuctionLotId.ToString()), "AuctionLotId");
        }
    }
}
