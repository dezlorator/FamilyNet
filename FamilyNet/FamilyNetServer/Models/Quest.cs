using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        [BindNever]
        public bool IsDeleted { get; set; } = true;
    }
}
