using FamilyNetServer.DTO;
using System;

namespace FamilyNetServer.Validators
{
    public interface IDonationItemValidator
    {
        bool IsValid(DonationItemDTO donationItemDTO);
    }
}
