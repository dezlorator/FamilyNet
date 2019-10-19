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
    public class ServerRepresentativesDownloader : ServerDataDownloader<RepresentativeDTO>
    {
        public override async Task<HttpStatusCode> CretePostAsync(string url, RepresentativeDTO dto, Stream streamFile, string fileName)
        {
            var statusCode = HttpStatusCode.BadRequest;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFormData(dto, streamFile, fileName, formDataContent);

                var msg = await httpClient.PostAsync(url, formDataContent);
                statusCode = msg.StatusCode;

                if (streamFile != null)
                {
                    streamFile.Close();
                }

                return statusCode;
            }
        }

        public override async Task<HttpStatusCode> СreatetePutAsync(string url, RepresentativeDTO dto, Stream streamFile, string fileName)
        {
            var statusCode = HttpStatusCode.BadRequest;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFormData(dto, streamFile, fileName, formDataContent);

                var msg = await httpClient.PutAsync(url, formDataContent);
                statusCode = msg.StatusCode;

                if (streamFile != null)
                {
                    streamFile.Close();
                }
            }

            return statusCode;
        }

        private static void BuildMultipartFormData(RepresentativeDTO dto,
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

            formDataContent.Add(new StringContent(dto.Name), "Name");
            formDataContent.Add(new StringContent(dto.Patronymic), "Patronymic");
            formDataContent.Add(new StringContent(dto.Surname), "Surname");
            formDataContent.Add(new StringContent(dto.Rating.ToString()), "Rating");
            formDataContent.Add(new StringContent(dto.Birthday.ToString()), "Birthday");
            formDataContent.Add(new StringContent(dto.ChildrenHouseID.ToString()),
                                                  "ChildrenHouseID");
        }
    }
}
