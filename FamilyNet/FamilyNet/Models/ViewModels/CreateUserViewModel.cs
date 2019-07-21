using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models.ViewModels
{
    public class CreateUserViewModel
    {
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
    }
}
