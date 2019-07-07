using System.Collections.Generic;

namespace FamilyNet.Models
{
    public class DonationItem : BaseItem
    {
        public ICollection<DonationItemType> DonationItemType { get; set; }
    }
}