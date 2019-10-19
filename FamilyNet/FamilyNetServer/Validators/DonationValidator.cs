using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.DTO;

namespace FamilyNetServer.Validators
{
    public class DonationValidator: IDonationValidator
    {
        public bool IsValid(DonationDTO donationDTO)
        {
            return (donationDTO.OrphanageID > 0 &&
                    donationDTO.CharityMakerID > 0 &&
                    donationDTO.DonationItemID > 0);
        }
    }
}
