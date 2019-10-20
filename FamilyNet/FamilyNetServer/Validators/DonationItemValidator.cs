using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.DTO;

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
