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
            IAddress adressSender = sender as IAddress;
            base.CopyState(sender);
            this.Address.CopyState(adressSender.Address);
        }
    }
}
