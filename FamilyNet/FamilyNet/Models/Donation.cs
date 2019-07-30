using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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


        [BindNever]
        public bool IsDeleted { get; set; } = false;

        public enum DonationStatus
        {
            Sended = 1,
            Aproved,
            Taken,
        }
    }
}