﻿using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class Volunteer : Person, IAddress
    {

        public int? AddressID { get; set; }

        [Display(Name="Адрес")]
        public virtual Address Address { get; set; }

        public virtual IEnumerable<Donation> Donations { get; set; }

        public override void CopyState(Person sender)
        {
            base.CopyState(sender);
            IAddress adressSender = sender as IAddress;
            this.Address.CopyState(adressSender.Address);
        }
    }
}
