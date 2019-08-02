using System.Collections.Generic;


namespace FamilyNet.Models
{
    public class DonationItem : BaseItem
    {
        public virtual ICollection<DonationItemType> DonationItemTypes { get; set; }
    }
}