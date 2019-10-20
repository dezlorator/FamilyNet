using DataTransferObjects;

namespace FamilyNet.Models.ViewModels
{
    public class DonationCreateViewModel
    {
        public DonationDetailDTO Donation { get; set; }

        public DonationItemDTO DonationItem { get; set; }
    }
}
