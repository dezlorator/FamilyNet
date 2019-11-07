﻿using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.ViewModels
{
    public class PurchaseBuyViewModel
    {
        public AuctionLotDTO AuctionLot { get; set; }

        public DonationItemDTO Item { get; set; }

        public PurchaseDTO Purchase { get; set; }

    }
}
