using FamilyNetServer.DTO;
using System;

namespace FamilyNetServer.Validators
{
    public class ChildValidator : IChildValidator
    {
        public bool IsValid(ChildDTO childDTO)
        {
            if (childDTO.Birthday == null ||
                String.IsNullOrEmpty(childDTO.Name) ||
                String.IsNullOrEmpty(childDTO.Surname) ||
                childDTO.OrphanageID < 0)
            {
                return false;
            }

            return true;
        }
    }
}
