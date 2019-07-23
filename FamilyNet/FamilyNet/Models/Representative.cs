using FamilyNet.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
{
    public class Representative : Person
    {
        public int OrphanageID { get; set; }
        //[Required]
        [Display(Name="Детдом")]
        public virtual Orphanage Orphanage { get; set; }

        public static void CopyState(Representative receiver, Representative sender)
        {
            receiver.FullName.Name = sender.FullName.Name;
            receiver.FullName.Surname = sender.FullName.Surname;
            receiver.FullName.Patronymic = sender.FullName.Patronymic;

            receiver.Avatar = sender.Avatar;
            receiver.Rating = sender.Rating;
            receiver.Birthday = sender.Birthday;

            receiver.Orphanage = sender.Orphanage;
        }
    }
}