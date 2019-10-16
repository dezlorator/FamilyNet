using DataTransferObjects;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class ServerChildrenDownloader : ServerDataDownLoader<ChildDTO>
    {

        public override async Task<HttpStatusCode> СreatetePostAsync(string url,
                                                            ChildDTO dto,
                                                            Stream streamFile,
                                                            string fileName)
        {
            var statusCode = HttpStatusCode.BadRequest;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                if (streamFile != null && streamFile.Length > 0)
                {
                    var image = new StreamContent(streamFile, (int)streamFile.Length);
                    formDataContent.Add(image, "Avatar", fileName);
                }

                formDataContent.Add(new StringContent(dto.Name), "Name");
                formDataContent.Add(new StringContent(dto.Patronymic), "Patronymic");
                formDataContent.Add(new StringContent(dto.Surname), "Surname");
                formDataContent.Add(new StringContent(dto.Birthday.ToString()), "Birthday");
                formDataContent.Add(new StringContent(dto.ChildrenHouseID.ToString()),
                                                      "ChildrenHouseID");

                var msg = await httpClient.PostAsync(url, formDataContent);
                statusCode = msg.StatusCode;

                if (streamFile != null)
                {
                    streamFile.Close();
                }
            }

            return statusCode;
        }
    }
}