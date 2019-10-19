using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DataTransferObjects
{
    public class DonationDTO
    {
        public int ID { get; set; }
        public string City { get; set; }
        public int? OrphanageID { get; set; }
        public int? CharityMakerID { get; set; }
        public int? DonationItemID { get; set; }
        public IFormFile OrphanageAvatar { get; set; }
        public string PathToAvatar { get; set; }
        public string Status { get; set; }
        public DateTime LastDateWhenStatusChanged { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public IEnumerable<int> Types { get; set; }
    }
}
