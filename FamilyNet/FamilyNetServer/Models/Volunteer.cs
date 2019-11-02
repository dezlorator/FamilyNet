using FamilyNetServer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models
{
    public class Volunteer : Person, IAddress
    {

        public int? AddressID { get; set; }

        [Display(Name="Адресс")]
        public virtual Address Address { get; set; }

        public override void CopyState(Person sender)
        {
            base.CopyState(sender);
            IAddress adressSender = sender as IAddress;
            this.Address.CopyState(adressSender.Address);
        }
    }
}