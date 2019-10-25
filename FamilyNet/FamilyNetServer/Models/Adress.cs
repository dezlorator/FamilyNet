using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models
{
    public class Address : IEntity
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите страну")]
        [Display(Name = "Cтрана")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите название области")]
        [Display(Name = "Область")]
        public string Region { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите название города")]
        [Display(Name = "Город")]
        public string City { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите название улицы")]
        [Display(Name = "Улица")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите номер дома")]
        [Display(Name = "Дом")]
        public string House { get; set; }

        [BindNever]
        public bool IsDeleted { get; set; } = false;

        public virtual void CopyState(Address sender)
        {
            City = sender.City;
            Country = sender.Country;
            House = sender.House;
            Region = sender.Region;
            Street = sender.Street;
        }
    }
}
