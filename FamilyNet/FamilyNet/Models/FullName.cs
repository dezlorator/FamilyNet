using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class FullName
    {
        public int ID { get; set; }
        public string Surname { get; set; }
        [Required]
        public string Name { get; set; }
        public string Patronymic { get; set; }
    }
}
