using FamilyNetServer.Enums;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace FamilyNetServer.Models
{
    public class Feedback : IEntity
    {
        public int ID { get; set; }
        [Display(Name = "Feedback message")]
        public string Message { get; set; }
        public string Image { get; set; }
        public int DonationId { get; set; }
        [Display(Name = "Receiver role")]
        public ReceiverRole ReceiverRole { get; set; }
        [Display(Name = "Rating")]
        public double Rating { get; set; } 
        public virtual Donation Donation { get; set; }
        public bool IsDeleted { get; set; }
    }
}
