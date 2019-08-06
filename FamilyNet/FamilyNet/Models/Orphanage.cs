using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class Orphanage : IEntity
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите название")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        public int? AdressID { get; set; }
                
        [Display(Name = "Адрес")]
        public virtual Address Adress { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите Рейтинг")]
        [Display(Name = "Рейтинг")]
        public float Rating { get; set; }

        [Display(Name = "Фото")]
        public string Avatar { get; set; }

        [Display(Name = "Представители")]
        public virtual ICollection<Representative> Representatives { get; set; }
                
        [Display(Name = "Дети")]
        public virtual ICollection<Orphan> Orphans { get; set; }

        public virtual ICollection<Donation> Donations { get; set; }

        public float? MapCoordX { get; set; }

        public float? MapCoordY { get; set; }

        public virtual void CopyState(Orphanage sender)
        {
            Name = sender.Name;
            Rating = sender.Rating;
            Avatar = sender.Avatar;
            Adress.CopyState(sender.Adress);
        }
    }
    public enum SortStateOrphanages // TODO : AlPa -> rewrite this byte
    {
        NameAsc,
        NameDesc,
        AddressAsc,
        AddressDesc,
        RatingAsc,
        RatingDesc
    }
}
