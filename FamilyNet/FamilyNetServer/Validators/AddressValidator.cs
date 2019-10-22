using FamilyNetServer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Validators
{
    public class AddressValidator : IValidator<AddressDTO>
    {
        public bool IsValid(AddressDTO addressDTO)
        {
            return !String.IsNullOrEmpty(addressDTO.Country) ||
                !String.IsNullOrEmpty(addressDTO.Region) ||
                !String.IsNullOrEmpty(addressDTO.City) ||
                !String.IsNullOrEmpty(addressDTO.Street) ||
                !String.IsNullOrEmpty(addressDTO.House);
        }
    }
}
