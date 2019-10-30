using System;
using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public class RepresentativeValidator : IRepresentativeValidator
    {
        public bool IsValid(RepresentativeDTO representativeDTO)
        {
            if (isValidDate(representativeDTO.Birthday) ||
                String.IsNullOrEmpty(representativeDTO.Name) ||
                String.IsNullOrEmpty(representativeDTO.Surname) ||
                representativeDTO.ChildrenHouseID < 0)
            {
                return false;
            }

            return true;
        }

        private bool isValidDate(DateTime birthday)
        {
            var maxAge = 90;

            if (birthday.Year > (DateTime.Now.Year - maxAge))
            { 
                return false;
            }

            return true;
        }
    }
}