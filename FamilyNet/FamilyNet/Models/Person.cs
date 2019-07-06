using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class Person
    {
        public int ID { get; set; }
        [Required]
        public FullName FullName { get; set; }
        public DateTime Birthday { get; set; }
        public Adress Address { get; set; }
        public Contacts Contacts { get; set; }
        public float Rating { get; set; }
        //public byte[] Avatar { get; set; }
    }
}
