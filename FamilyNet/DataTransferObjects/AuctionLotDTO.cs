using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferObjects
{
    public class AuctionLotDTO
    {
        public int ID { get; set; }

        public DateTime DateAdded { get; set; }

        public int? AuctionLotItemID { get; set; }

        public int? OrphanID { get; set; }

        public IFormFile Avatar { get; set; }

        public string PhotoParth { get; set; }

        public int Quantity { get; set; }

        public string Status { get; set; }

    }
}
