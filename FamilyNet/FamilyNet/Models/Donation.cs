using FamilyNet.Models.Interfaces;
using System;

namespace FamilyNet.Models
{
    public class Donation : IEntity
    {
        public int ID { get; set; }

        public int? DonationItemID { get; set; }

        public virtual DonationItem DonationItem { get; set; }  

        public bool IsRequest { get; set; }

        public int? CharityMakerID { get; set; }

        public virtual CharityMaker CharityMaker { get; set; }

        public int? OrphanageID { get; set; }

        public virtual Orphanage Orphanage { get; set; }

        public DonationStatus Status { get; set; }

        public DateTime LastDateWhenStatusChanged { get; set; }

        public enum DonationStatus
        {
            Sended = 1,
            Aproved,
            Taken,
        }
    }
}