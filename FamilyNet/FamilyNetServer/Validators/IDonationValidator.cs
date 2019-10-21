using FamilyNetServer.DTO;
using System;

namespace FamilyNetServer.Validators
{
    public interface IDonationValidator
    {
        bool IsValid(DonationDTO donationDTO);
    }
}
