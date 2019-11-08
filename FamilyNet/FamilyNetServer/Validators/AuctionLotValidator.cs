using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Validators
{
    public class AuctionLotValidator : IValidator<AuctionLotDTO>
    {
        public bool IsValid(AuctionLotDTO auctionDTO)
        {
            return (auctionDTO.Quantity > 0 ||
                auctionDTO.OrphanID > 0 ||
                auctionDTO.AuctionLotItemID > 0 ||
                auctionDTO.DateStart >= DateTime.MinValue ||
                auctionDTO.DateEnd >= DateTime.MinValue);
        }
    }
}
