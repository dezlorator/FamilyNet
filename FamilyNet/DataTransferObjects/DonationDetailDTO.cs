using System.Collections.Generic;

namespace DataTransferObjects
{
    public class DonationDetailDTO : DonationDTO
    {
        public IEnumerable<string> Categories { get; set; }
        public string OrphanageName { get; set; }
        public string OrphanageCity { get; set; }
        public string OrphanageStreet { get; set; }
        public string OrphanageHouse { get; set; }
        public float OrphanageRating { get; set; }
    }
}
