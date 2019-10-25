using DataTransferObjects;
using System;

namespace FamilyNetServer.Validators
{
    public interface IDonationItemValidator
    {
        bool IsValid(DonationItemDTO donationItemDTO);
    }
}
