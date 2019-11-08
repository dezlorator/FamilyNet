using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferObjects
{
    public class PurchaseDTO
    {
        public int ID { get; set; }

        public DateTime Date { get; set; }

        public string UserId { get; set; }

        public int Quantity { get; set; }

        public float Paid { get; set; }

        public int AuctionLotId { get; set; }

    }
}
