using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.ViewModels
{
    public class AuctionLotCreateViewModel
    {
        public AuctionLotDTO  AuctionLot { get; set; }

        public DonationItemDTO Item { get; set; }
    }
}
