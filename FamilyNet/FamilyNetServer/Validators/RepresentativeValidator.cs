using System;
using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public class RepresentativeValidator : IRepresentativeValidator
    {
        public bool IsValid(RepresentativeDTO representativeDTO)
        {
            if (representativeDTO.Birthday == null ||
                String.IsNullOrEmpty(representativeDTO.Name) ||
                String.IsNullOrEmpty(representativeDTO.Surname) ||
                representativeDTO.ChildrenHouseID < 0)
            {
                return false;
            }

            return true;
        }
    }
}