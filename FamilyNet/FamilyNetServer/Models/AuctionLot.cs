﻿using FamilyNetServer.Enums;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace FamilyNetServer.Models
{
    public class AuctionLot : IEntity
    {
        public int ID { get; set; }

        public DateTime DateAdded { get; set; }

        public int? AuctionLotItemID { get; set; }

        public virtual AuctionLotItem AuctionLotItem { get; set; }

        public int? OrphanID { get; set; }

        public virtual Orphan Orphan { get; set; }

        [BindNever]
        public bool IsDeleted { get; set; } = false;

        public string Avatar { get; set; }

        public int Quantity { get; set; }

        public AuctionLotStatus Status { get; set; }
    }
}