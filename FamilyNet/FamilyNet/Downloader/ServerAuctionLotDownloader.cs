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
    public class ServerAuctionLotDownloader : ServerDataDownloader<AuctionLotDTO>
    {        

        public ServerAuctionLotDownloader(IHttpAuthorizationHandler authorizationHandler)
            : base(authorizationHandler) { }


        public override async Task<HttpStatusCode> CreatePostAsync(string url, AuctionLotDTO dto, 
            Stream file, string fileName, ISession session)
        {
            var statusCode = HttpStatusCode.BadRequest;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFormData(dto, file, formDataContent);
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

        public override async Task<HttpStatusCode> CreatePutAsync(string url, AuctionLotDTO dto,
            Stream file, string fieName, ISession session)
        {
            var statusCode = HttpStatusCode.BadRequest;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFormData(dto, file, formDataContent);
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

        public async override Task<IEnumerable<AuctionLotDTO>> GetAllAsync(string url,
                                                      ISession session)
        {
            AuctionLotFilterDTO objs = null;

            try
            {
                HttpResponseMessage response = null;

                using (var httpClient = new HttpClient())
                {
                    _authorizationHandler.AddTokenBearer(session, httpClient);
                    response = await httpClient.GetAsync(url);
                }

                var json = await response.Content.ReadAsStringAsync();
                objs = JsonConvert.DeserializeObject<AuctionLotFilterDTO>(json);
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

            return objs.AuctionLotDTOs;
        }

        private static void BuildMultipartFormData(AuctionLotDTO dto,
                                                  Stream streamFile,
                                                  MultipartFormDataContent formDataContent)
        {
            if (streamFile != null && streamFile.Length > 0)
            {
                var image = new StreamContent(streamFile, (int)streamFile.Length);
                formDataContent.Add(image, "Avatar", dto.Avatar.FileName);
            }

            if (dto.ID > 0)
            {
                formDataContent.Add(new StringContent(dto.ID.ToString()), "ID");
            }

            formDataContent.Add(new StringContent(dto.AuctionLotItemID.ToString()), "AuctionLotItemID");
            if (dto.DateAdded > DateTime.MinValue)
            {
                formDataContent.Add(new StringContent(dto.DateAdded.ToString()), "DateAdded");
            }
            formDataContent.Add(new StringContent(dto.OrphanID.ToString()), "OrphanID");
            formDataContent.Add(new StringContent(dto.Quantity.ToString()), "Quantity");
            formDataContent.Add(new StringContent(dto.Status.ToString()), "Status");
        }

    }
}
