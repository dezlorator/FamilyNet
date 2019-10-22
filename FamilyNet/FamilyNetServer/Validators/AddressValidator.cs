using DataTransferObjects;
using System;

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
