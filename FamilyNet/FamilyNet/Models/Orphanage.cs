using FamilyNet.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class Orphanage : IEntity
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }
        public int AddressID { get; set; }
        public virtual Address Address { get; set; }
        public float Rating { get; set; }
        public string Avatar { get; set; }

        public virtual ICollection<Representative> Representatives { get; set; }
        public virtual ICollection<Orphan> OrphansIds { get; set; }

        public static void CopyState(Orphanage receiver, Orphanage sender)
        {
            receiver.Name = sender.Name;
            receiver.Rating = sender.Rating;
            receiver.Avatar = receiver.Avatar;
            receiver.Address.City = sender.Address.City;
            receiver.Address.Country = sender.Address.Country;
            receiver.Address.House = sender.Address.House;
            receiver.Address.Region = sender.Address.Region;
            receiver.Address.Street = sender.Address.Street;
        }
    }
}
