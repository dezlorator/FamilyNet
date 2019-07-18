using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class Person : IEntity
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please enter Full Name")]
        public virtual FullName FullName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]      
        public virtual DateTime Birthday { get; set; }
        public virtual  Adress Address { get; set; }
        [Required]
        public virtual Contacts Contacts { get; set; }
        public float Rating { get; set; }
        //public byte[] Avatar { get; set; }
    }
}
