using DataTransferObjects;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class ServerDonationsDownloader : ServerSimpleDataDownloader<DonationDetailDTO>
    {
        public override async Task<HttpStatusCode> СreatetePostAsync(string url,
                                                               DonationDetailDTO dto)
        {
            var statusCode = HttpStatusCode.BadRequest;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFormData(dto, formDataContent);

                var msg = await httpClient.PostAsync(url, formDataContent);
                statusCode = msg.StatusCode;
            }

            return statusCode;
        }

        public override async Task<HttpStatusCode> СreatePutAsync(string url,
                                                                  DonationDetailDTO donationDTO)
        {
            {
                var statusCode = HttpStatusCode.BadRequest;

                using (var httpClient = new HttpClient())
                using (var formDataContent = new MultipartFormDataContent())
                {
                    BuildMultipartFormData(donationDTO, formDataContent);

                    var msg = await httpClient.PutAsync(url, formDataContent);
                    statusCode = msg.StatusCode;
                }

                return statusCode;
            }
        }

        private static void BuildMultipartFormData(DonationDTO dto,
                                                   MultipartFormDataContent formDataContent)
        {
            if (dto.ID > 0)
            {
                formDataContent.Add(new StringContent(dto.ID.ToString()), "ID");
            }

            formDataContent.Add(new StringContent(dto.OrphanageID.ToString()), "OrphanageID");
            formDataContent.Add(new StringContent(dto.Status.ToString()), "Status");
            formDataContent.Add(new StringContent(dto.DonationItemID.ToString()), "DonationItemID");
            formDataContent.Add(new StringContent(dto.CharityMakerID.ToString()), "CharityMakerID");
        }
    }
}

