using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects
{
    public class ChildrenHouseDTO
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите название")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        public int? AdressID { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите Рейтинг")]
        [Display(Name = "Рейтинг")]
        public float Rating { get; set; }

        [Display(Name = "Фото")]
        public IFormFile Avatar { get; set; }

        public int? LocationID { get; set; }

        public string PhotoPath { get; set; }

    }
}
