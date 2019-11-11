using DataTransferObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.ViewModels
{
    public class CreateFeedbackViewModel
    {
        public FeedbackDTO feedbackDTO { get; set; }
        public List<string> Roles { get; set; }
        public string Role { get; set; }
        public IFormFile Avatar { get; set; }
    }
}
