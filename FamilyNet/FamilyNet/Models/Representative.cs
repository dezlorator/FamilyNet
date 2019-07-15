using System.ComponentModel.DataAnnotations;

namespace FamilyNet.Models
{
    public class Representative : Person
    {
        [Required]
        public virtual Orphanage Orphanage { get; set; }

        public static void CopyState(Representative receiver, Representative sender)
        {
            receiver.FullName.Name = sender.FullName.Name;
            receiver.FullName.Patronymic = sender.FullName.Patronymic;
            receiver.FullName.Surname = sender.FullName.Surname;

            receiver.Rating = sender.Rating;
            receiver.Birthday = receiver.Birthday;

            receiver.Contacts.Email = sender.Contacts.Email; 
            receiver.Contacts.Phone = sender.Contacts.Phone;

            receiver.Address.City = sender.Address.City;
            receiver.Address.Country = sender.Address.Country;
            receiver.Address.House = sender.Address.House;
            receiver.Address.Region = sender.Address.Region;
            receiver.Address.Street = sender.Address.Street;
        }
    }
}