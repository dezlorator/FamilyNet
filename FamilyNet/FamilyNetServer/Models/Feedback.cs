using FamilyNetServer.Enums;
using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using DataTransferObjects.Enums;

namespace FamilyNetServer.Models
{
    public class Feedback : IEntity
    {
        public int ID { get; set; }
        [Display(Name = "Feedback message")]
        public string Message { get; set; }
        [Display(Name = "Date, when message was sent")]
        public DateTime Time { get; set; }
        public string Image { get; set; }
        public int DonationId { get; set; }
        [Display(Name = "Receiver role")]
        public UserRole ReceiverRole { get; set; }
        public int? ReceiverId { get; set; }
        public UserRole SenderRole { get; set; }
        public int? SenderId { get; set; }
        [Display(Name = "Rating")]
        public double Rating { get; set; } 
        public bool IsDeleted { get; set; }
    }
}
