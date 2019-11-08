using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class Purchase 
    {
        public int ID { get; set; }

        public DateTime Date { get; set; }

        public Guid UserId { get; set; }

        public int Quantity { get; set; }

        public float Paid { get; set; }

        public int AuctionLotId { get; set; }

        public virtual AuctionLot AuctionLot { get; set; }

        public bool IsDeleted { get; set; }
    }
}
