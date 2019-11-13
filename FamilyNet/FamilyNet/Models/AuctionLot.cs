using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
{
    public class AuctionLot
    {
        public int ID { get; set; }

        public DateTime DateStart { get; set; }

        public int? AuctionLotItemID { get; set; }

        public virtual AuctionLotItem AuctionLotItem {get;set;}

        public int? OrphanID { get; set; }

        public virtual Orphan Orphan { get; set; }

        [BindNever]
        public bool IsDeleted { get; set; } = false;

        public string Avatar { get; set; }

        [Required(ErrorMessage = "Please enter quantity")]
        public int Quantity { get; set; }

        public string Status { get; set; }
    }
}