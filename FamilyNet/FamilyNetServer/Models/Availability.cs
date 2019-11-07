using FamilyNetServer.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyNetServer.Models
{
    public class Availability : IEntity
    {
        public int ID { get; set; }
        public int VolunteerID { get; set; }

        //public Quest Quest { get; set;}

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        [Column(TypeName = "time")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan FreeHours { get; set; } 

        public bool isTaken { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}
