using FamilyNetServer.DTO;
using System;

namespace FamilyNetServer.Validators
{
    public class VolunteerValidator : IVolunteerValidator
    {
        public bool IsValid(VolunteerDTO volunteerDTO)
        {
            if (volunteerDTO.Birthday == null ||
                String.IsNullOrEmpty(volunteerDTO.Name) ||
                String.IsNullOrEmpty(volunteerDTO.Surname) ||
                volunteerDTO.AddressID < 0)
            {
                return false;
            }

            return true;
        }
    }
}
