using System.Collections.Generic;

namespace FamilyNet.Models
{
    public class AuctionLotItem : BaseItem
    {
        public ICollection<AuctionLotItemType> AuctionLotItemType { get; set; } 
        
    }
}