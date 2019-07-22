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
        //[Required(ErrorMessage = "Please enter a name")]
        [Display(Name = "Название")]
        public string Name { get; set; }
        public int? AdressID { get; set; }

        [Display(Name = "Адрес")]
        public virtual Address Adress { get; set; }
        [Display(Name = "Рейтинг")]
        public float Rating { get; set; }
        [Display(Name = "Фото")]
        public string Avatar { get; set; }

        [Display(Name = "Представители")]
        public virtual ICollection<Representative> Representatives { get; set; }

        [Display(Name = "Дети")]
        public virtual ICollection<Orphan> Orphans { get; set; }


        public static void CopyState(Orphanage receiver, Orphanage sender)
        {
            receiver.Name = sender.Name;
            receiver.Rating = sender.Rating;
            receiver.Avatar = sender.Avatar;
            receiver.Adress.City = sender.Adress.City;
            receiver.Adress.Country = sender.Adress.Country;
            receiver.Adress.House = sender.Adress.House;
            receiver.Adress.Region = sender.Adress.Region;
            receiver.Adress.Street = sender.Adress.Street;
        }
    }
    public enum SortStateOrphanages
    {
        NameAsc,
        NameDesc,
        AddressAsc,
        AddressDesc,
        RatingAsc,
        RatingDesc
    }
}
