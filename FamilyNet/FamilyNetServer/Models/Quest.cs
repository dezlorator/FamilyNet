using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FamilyNetServer.Models.Interfaces;

namespace FamilyNetServer.Models
{
    public class Quest : IEntity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? DonationID { get; set; }
        public virtual Donation Donation { get; set; }
        public int? VolunteerID { get; set; }
        public virtual Volunteer Volunteer { get; set; }
        public string Description { get; set; }
        public QuestStatus Status { get; set; } = QuestStatus.ToDo;
        public bool IsDeleted { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }

        [DataType(DataType.Time)]
        [Column(TypeName = "time")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan Hours { get; set; }
    }
}
