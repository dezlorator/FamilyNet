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
        public override async Task<HttpResponseMessage> CreatePostAsync(string url,
                                                               DonationDetailDTO dto)
        {
            HttpResponseMessage msg = null;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFormData(dto, formDataContent);

                msg = await httpClient.PostAsync(url, formDataContent);
            }

            return msg;
        }

        public override async Task<HttpResponseMessage> CreatePutAsync(string url,
                                                                  DonationDetailDTO donationDTO)
        {
            HttpResponseMessage msg = null;

            using (var httpClient = new HttpClient())
            using (var formDataContent = new MultipartFormDataContent())
            {
                BuildMultipartFormData(donationDTO, formDataContent);

                msg = await httpClient.PutAsync(url, formDataContent);
            }

            return msg;
        }

        private static void BuildMultipartFormData(DonationDTO dto,
                                                   MultipartFormDataContent formDataContent)
        {
            if (dto.ID > 0)
            {
                formDataContent.Add(new StringContent(dto.ID.ToString()), "ID");
            }

            if (dto.OrphanageID != null)
            {
                formDataContent.Add(new StringContent(dto.OrphanageID.ToString()), "OrphanageID");
            }

            if (!string.IsNullOrEmpty(dto.Status))
            {
                formDataContent.Add(new StringContent(dto.Status.ToString()), "Status");
            }

            if (dto.DonationItemID != null)
            {
                formDataContent.Add(new StringContent(dto.DonationItemID.ToString()), "DonationItemID");
            }

            if (dto.CharityMakerID != null)
            {
                formDataContent.Add(new StringContent(dto.CharityMakerID.ToString()), "CharityMakerID");
            }
        }
    }
}

