using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FamilyNetServer.Models
{
    public class Orphan : Person
    {
        public int? OrphanageID { get; set; }

        [Display(Name = "Детский дом")]
        public virtual Orphanage Orphanage { get; set; }

        public virtual ICollection<AuctionLot> AuctionLots { get; set; }

        public bool Confirmation { get; set; }

        public bool ChildInOrphanage { get; set; }

        public override void CopyState(Person sender)
        {
            base.CopyState(sender);
            Orphan orphanSended = sender as Orphan;
            Avatar = orphanSended.Avatar;
            Orphanage = orphanSended.Orphanage;
            Confirmation = orphanSended.Confirmation;
            ChildInOrphanage = orphanSended.ChildInOrphanage;
        }
    }
}