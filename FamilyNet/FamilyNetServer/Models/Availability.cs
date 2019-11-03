using FamilyNetServer.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FamilyNetServer.Models
{
    public class Availability : IEntity
    {
        public int ID { get; set; }
        public string VolunteerID { get; set; }
        public DateTime Date { get; set; }
        public DateTime FromHour { get; set; }
        public TimeSpan VolunteerHours { get; set; }
        [NotMapped]
        public bool IsDeleted { get; set; }
    }
}
