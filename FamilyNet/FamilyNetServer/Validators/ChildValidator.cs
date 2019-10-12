using DataTransferObjects;
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
                childDTO.ChildrenHouseID < 0)
            {
                return false;
            }

            return true;
        }
    }
}
