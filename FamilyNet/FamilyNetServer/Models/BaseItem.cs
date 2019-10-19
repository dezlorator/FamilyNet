﻿using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models
{
    public class BaseItem : IEntity
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        //[Required]
        //[Range(0.01, double.MaxValue,
            //ErrorMessage = "Please enter a positive price")]
        public float Price { get; set; }

        [BindNever]
        public bool IsDeleted { get; set; } = false;

        [Display(Name = "Категории")]
        public virtual ICollection<TypeBaseItem> TypeBaseItem { get; set; }
    }
}
