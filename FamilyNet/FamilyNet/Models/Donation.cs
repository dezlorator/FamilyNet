using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyNet.Models
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

        [Display(Name = "Категория")]
        [NotMapped]
        public int idDonationItem { get; set; }
    }
}