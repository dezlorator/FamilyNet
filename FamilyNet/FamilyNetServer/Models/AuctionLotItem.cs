using System.Collections.Generic;

namespace FamilyNetServer.Models
{
    public class AuctionLotItem : BaseItem
    {
        public virtual ICollection<AuctionLotItemType> AuctionLotItemTypes { get; set; }
    }
}