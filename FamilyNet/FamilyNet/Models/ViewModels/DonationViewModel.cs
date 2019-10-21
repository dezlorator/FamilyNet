using DataTransferObjects;

namespace FamilyNet.Models.ViewModels
{
    public class DonationViewModel
    {
        public DonationDetailDTO Donation { get; set; }

        public DonationItemDTO DonationItem { get; set; }
    }
}
