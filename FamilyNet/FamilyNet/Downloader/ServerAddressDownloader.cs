using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class ServerAddressDownloader : ServerDataDownLoader<AddressDTO>
    {
        public override async Task<HttpStatusCode> СreatePostAsync(string url,
                                                           AddressDTO dto,
                                                           Stream streamFile,
                                                           string fileName)
        {
            var statusCode = HttpStatusCode.BadRequest;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFprmData(dto, streamFile, fileName, formDataContent);

                var msg = await httpClient.PostAsync(url, formDataContent);
                statusCode = msg.StatusCode;

                if (streamFile != null)
                {
                    streamFile.Close();
                }
            }

            return statusCode;
        }

        public override async Task<HttpStatusCode> СreatePutAsync(string url,
                                                              AddressDTO dto,
                                                              Stream streamFile,
                                                              string fileName)
        {
            var statusCode = HttpStatusCode.BadRequest;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFprmData(dto, streamFile, fileName, formDataContent);

                var msg = await httpClient.PutAsync(url, formDataContent);
                statusCode = msg.StatusCode;

                if (streamFile != null)
                {
                    streamFile.Close();
                }
            }

            return statusCode;
        }

        private static void BuildMultipartFprmData(AddressDTO dto,
                                                   Stream streamFile,
                                                   string fileName,
                                                   MultipartFormDataContent formDataContent)
        {
            if (streamFile != null && streamFile.Length > 0)
            {
                var image = new StreamContent(streamFile, (int)streamFile.Length);
                formDataContent.Add(image, "Avatar", fileName);
            }

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
