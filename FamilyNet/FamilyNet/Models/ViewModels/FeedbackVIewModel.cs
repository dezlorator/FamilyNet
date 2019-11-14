using DataTransferObjects;
using DataTransferObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.ViewModels
{
    public class FeedbackVIewModel
    {
        public FeedbackDTO FeedbackDTO { get; set; }
        public SNPDTO ReceiverFio { get; set; }
        public SNPDTO SenderFio { get; set; }
    }
}
