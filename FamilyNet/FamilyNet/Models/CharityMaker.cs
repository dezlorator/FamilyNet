using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class CharityMaker : Person, IAddress
    {
        public virtual ICollection<Donation> Donations { get; set; }

        public int? AddressID { get; set; }

        [Display(Name = "Адрес")]
        public virtual Address Address { get; set; }
        
        public override void CopyState(Person sender)
        {
            IAddress adressSender = sender as IAddress;
            base.CopyState(sender);
            if (sender.Avatar != string.Empty && sender.Avatar != null)
            {
                Avatar = sender.Avatar;
            }
            this.Address.CopyState(adressSender.Address);
        }
    }
}
