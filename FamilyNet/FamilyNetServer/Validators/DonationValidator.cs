using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public class DonationValidator: IDonationValidator
    {
        public bool IsValid(DonationDTO donationDTO)
        {
            return (donationDTO.OrphanageID == null || donationDTO.OrphanageID > 0 &&
                    donationDTO.CharityMakerID == null || donationDTO.CharityMakerID > 0 &&
                    donationDTO.DonationItemID == null || donationDTO.DonationItemID > 0);
        }
    }
}
