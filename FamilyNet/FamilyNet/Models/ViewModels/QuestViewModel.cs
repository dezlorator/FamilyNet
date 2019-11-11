using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects;

namespace FamilyNet.Models.ViewModels
{
    public class QuestViewModel
    {
        public QuestDTO Quest { get; set; }
        public DonationDTO Donation { get; set; }
    }
}
