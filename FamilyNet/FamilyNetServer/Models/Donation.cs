using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyNetServer.Models
{
    public class Donation : IEntity
    {
        public int ID { get; set; }

        public int? DonationItemID { get; set; }

        [Display(Name = "Потребность")]
        public virtual DonationItem DonationItem { get; set; }  

        public bool IsRequest { get; set; }

        public int? CharityMakerID { get; set; }

        [Display(Name = "Филантроп")]
        public virtual CharityMaker CharityMaker { get; set; }

        public int? OrphanageID { get; set; }

        [Display(Name = "Детский дом")]
        public virtual Orphanage Orphanage { get; set; }

        [Display(Name = "Статус")]
        public DonationStatus Status { get; set; }

        [Display(Name = "Последние изменения")]
        public DateTime LastDateWhenStatusChanged { get; set; }


        [BindNever]
        public bool IsDeleted { get; set; } = false;

        public enum DonationStatus
        {
            Sended = 1,
            Aproved,
            Taken,
        }

        public void CopyState(Donation sender)
        {            
            Donation donationSended = sender as Donation;
            Orphanage = donationSended.Orphanage;
            Status = donationSended.Status;
            LastDateWhenStatusChanged = donationSended.LastDateWhenStatusChanged;
            DonationItem = donationSended.DonationItem;            
        }

        [Display(Name = "Категория")]
        [NotMapped]
        public int idDonationItem { get; set; }
    }
}