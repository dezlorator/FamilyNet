using System;

namespace FamilyNet.Models
{
    public class AuctionLot
    {
        public int ID { get; set; }
        public AuctionLotItem AuctionLotItem { get; set; }
        public DateTime Date { get; set; }
    }
}