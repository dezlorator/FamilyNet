using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
{
    public class Orphan : Person
    {
        //[Required]
        public virtual Orphanage Orphanage { get; set; }
        public virtual ICollection<AuctionLot> AuctionLots { get; set; }

        public static void CopyState(Orphan receiver, Orphan sender)
        {
            receiver.FullName.Name = sender.FullName.Name;
            receiver.FullName.Surname = sender.FullName.Surname;
            receiver.FullName.Patronymic = sender.FullName.Patronymic;

            receiver.Rating = sender.Rating;
            receiver.Birthday = receiver.Birthday;

            receiver.Contacts.Phone = sender.Contacts.Phone;
            receiver.Contacts.Email = sender.Contacts.Email;

        }
    }
}