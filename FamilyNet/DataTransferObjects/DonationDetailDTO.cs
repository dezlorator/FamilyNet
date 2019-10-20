using System.Collections.Generic;

namespace DataTransferObjects
{
    public class DonationDetailDTO : DonationDTO
    {
        public string OrphanageStreet { get; set; }
        public string OrphanageHouse { get; set; }
        public float OrphanageRating { get; set; }
    }
}
