using System;
using FamilyNetServer.DTO;

namespace FamilyNetServer.Validators
{
    public class RepresentativeValidator : IRepresentativeValidator
    {
        public bool IsValid(RepresentativeDTO representativeDTO)
        {
            if (representativeDTO.Birthday == null ||
                String.IsNullOrEmpty(representativeDTO.Name) ||
                String.IsNullOrEmpty(representativeDTO.Surname) ||
                representativeDTO.OrphanageID < 0)
            {
                return false;
            }

            return true;
        }
    }
}
