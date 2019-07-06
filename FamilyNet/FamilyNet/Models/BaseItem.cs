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
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Range(0.01, double.MaxValue,
            ErrorMessage = "Please enter a positive price")]
        public float Price { get; set; }
    }
}
