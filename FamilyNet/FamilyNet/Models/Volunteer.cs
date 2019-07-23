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
        [Display(Name="Адресс")]
        public virtual Address Address { get; set; }

        public static void CopyState(Volunteer receiver, Volunteer sender)
        {
            receiver.FullName.Name = sender.FullName.Name;
            receiver.FullName.Surname = sender.FullName.Surname;
            receiver.FullName.Patronymic = sender.FullName.Patronymic;

            receiver.Rating = sender.Rating;
            receiver.Birthday = receiver.Birthday;


        }
    }
}
