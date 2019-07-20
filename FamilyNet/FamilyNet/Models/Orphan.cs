using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
{
    public class Orphan : Person
    {
        public int? OrphanageID { get; set; }
        //[Required]
        public virtual Orphanage Orphanage { get; set; }

        public virtual ICollection<AuctionLot> AuctionLots { get; set; }

        public bool Confirmation { get; set; }
        public bool ChildInOrphanage { get; set; }
    }
}