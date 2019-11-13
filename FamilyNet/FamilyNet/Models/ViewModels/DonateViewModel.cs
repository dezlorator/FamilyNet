using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects;

namespace FamilyNet.Models.ViewModels
{
    public class DonateViewModel
    {
        public DonationDetailDTO Donation { get; set; }

        public bool NeedHelp { get; set; }
    }
}
