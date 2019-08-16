using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class Address
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Будь ласка введіть країну")]
        [Display(Name = "Країна")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Будь ласка введіть назву області")]
        [Display(Name = "Область")]
        public string Region { get; set; }

        [Required(ErrorMessage = "Будь ласка введіть назву міста")]
        [Display(Name = "Місто")]
        public string City { get; set; }

        [Required(ErrorMessage = "Будь ласка введіть назву вулиці")]
        [Display(Name = "Вулиця")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Будь ласка введіть назву номер будинку")]
        [Display(Name = "Будинок")]
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
