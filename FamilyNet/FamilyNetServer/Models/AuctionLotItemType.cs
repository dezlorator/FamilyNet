namespace FamilyNetServer.Models
{
    public class AuctionLotItemType : BaseItemType
    {
        public int? ItemID { get; set; }

        public virtual AuctionLotItem Item { get; set; }
    }
}