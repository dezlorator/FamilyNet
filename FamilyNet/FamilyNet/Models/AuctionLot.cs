using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace FamilyNet.Models
{
    public class AuctionLot
    {
        public int ID { get; set; }

        public DateTime Date { get; set; }

        public int? AuctionLotItemID { get; set; }

        public virtual AuctionLotItem AuctionLotItem {get;set;}

        public int? OrphanID { get; set; }

        public virtual Orphan Orphan { get; set; }

        [BindNever]
        public bool IsDeleted { get; set; } = false;
    }
}