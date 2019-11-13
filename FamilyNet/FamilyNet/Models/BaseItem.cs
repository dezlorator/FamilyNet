using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class BaseItem 
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter desciption")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        //[Required]
        //[Range(0.01, double.MaxValue,
        //ErrorMessage = "Please enter a positive price")]
        [Required(ErrorMessage = "Please enter price")]
        public float Price { get; set; }

        [BindNever]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "Категория")]
        public virtual ICollection<TypeBaseItem> TypeBaseItem { get; set; }
    }
}
