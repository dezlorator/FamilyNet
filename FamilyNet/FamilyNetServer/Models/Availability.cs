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

        [DataType(DataType.Date)]
        [Column("FromHour", TypeName = "SmallDateTime")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FromHour { get; set; }

        [DataType(DataType.Time)]
        [Column(TypeName = "time")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan VolunteerHours { get; set; }
        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}
