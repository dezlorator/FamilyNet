using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public class PurchaseValidator : IValidator<PurchaseDTO>
    {
        public bool IsValid(PurchaseDTO purchaseDTO)
        {
            return (purchaseDTO.AuctionLotId > 0 ||
                purchaseDTO.Date >= DateTime.MinValue ||
                purchaseDTO.Paid > 0 ||
                purchaseDTO.Quantity > 0 ||
                purchaseDTO.UserId != null);
        }
    }
}
