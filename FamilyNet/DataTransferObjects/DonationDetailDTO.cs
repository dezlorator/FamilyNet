using System.Collections.Generic;

namespace DataTransferObjects
{
    public class DonationDetailDTO : DonationDTO
    {
        public string Street { get; set; }
        public string House { get; set; }
        public float Rating { get; set; }
    }
}
