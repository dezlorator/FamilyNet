using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.DTO
{
    public class DonationDetailDTO
    {
        public IEnumerable<string> Categories { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string OrphanageName { get; set; }
        public string OrphanageCity { get; set; }
        public string OrphanageStreet { get; set; }
        public string OrphanageHouse { get; set; }
        public float OrphanageRating { get; set; }
    }
}
