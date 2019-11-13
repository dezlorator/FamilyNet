using DataTransferObjects;
using System;

namespace FamilyNetServer.Validators
{
    public class VolunteerValidator : IVolunteerValidator
    {
        public bool IsValid(VolunteerDTO volunteerDTO)
        {
            if (String.IsNullOrEmpty(volunteerDTO.Name) ||
                String.IsNullOrEmpty(volunteerDTO.Surname) ||
                String.IsNullOrEmpty(volunteerDTO.Patronymic) ||
                volunteerDTO.AddressID < 0)
            {
                return false;
            }

            return true;
        }
    }
}
