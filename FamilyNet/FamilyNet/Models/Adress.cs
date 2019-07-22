using FamilyNet.Models.Interfaces;
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
    }
}
