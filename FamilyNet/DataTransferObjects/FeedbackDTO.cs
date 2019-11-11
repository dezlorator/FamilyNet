using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using DataTransferObjects.Enums;

namespace DataTransferObjects
{
    public class FeedbackDTO
    {
        public int ID { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public string ImagePath { get; set; }
        public int DonationId { get; set; }
        public UserRole ReceiverRole { get; set; }
        public int? ReceiverId { get; set; }
        public IFormFile Image { get; set; }
        public int SenderId { get; set; }
        public UserRole SenderRole { get; set; }
        public double Rating { get; set; }
        public bool IsDeleted { get; set; }
    }
}
