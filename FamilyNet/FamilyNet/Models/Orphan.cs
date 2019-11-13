using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
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
            Orphan orphanSent = sender as Orphan;
            Avatar = orphanSent.Avatar;
            Orphanage = orphanSent.Orphanage;
            Confirmation = orphanSent.Confirmation;
            ChildInOrphanage = orphanSent.ChildInOrphanage;
        }
    }
}