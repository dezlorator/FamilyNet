using System;
using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public class DonationItemValidator : IDonationItemValidator
    {
        public bool IsValid(DonationItemDTO itemDTO)
        {
            return itemDTO.Name != String.Empty &&
                   itemDTO.Description != String.Empty &&
                   itemDTO.Price > 0;
        }
    }
}
