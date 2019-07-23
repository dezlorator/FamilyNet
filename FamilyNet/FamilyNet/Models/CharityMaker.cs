using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class CharityMaker : Person, IAddress
    {
        public virtual ICollection<Donation> Donations { get; set; }

        public int? AddressID { get; set; }
        public virtual Address Address { get; set; }

        public override void CopyState(Person sender)
        {
            IAddress sender2 = sender as IAddress;
            base.CopyState(sender);
            Address.City = sender2.Address.City;
            Address.Country = sender2.Address.Country;
            Address.House = sender2.Address.House;
            Address.Region = sender2.Address.Region;
            Address.Street = sender2.Address.Street;

        }
    }
}
