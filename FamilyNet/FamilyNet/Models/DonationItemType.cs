namespace FamilyNet.Models
{
    public class DonationItemType : BaseItemType
    {
        public int? ItemID { get; set; }

        public virtual DonationItem Item { get; set; }
    }
}