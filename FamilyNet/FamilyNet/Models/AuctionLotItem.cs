using System.Collections.Generic;

namespace FamilyNet.Models
{
    public class AuctionLotItem : BaseItem
    {
        public virtual ICollection<AuctionLotItemType> AuctionLotItemType { get; set; } 
        
    }
}