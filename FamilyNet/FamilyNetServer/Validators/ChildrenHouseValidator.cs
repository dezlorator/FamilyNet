using DataTransferObjects;
using System;

namespace FamilyNetServer.Validators
{
    public class ChildrenHouseValidator : IValidator<ChildrenHouseDTO>
    {
        public bool IsValid(ChildrenHouseDTO childrenHouseDTO)
        {
            return !(String.IsNullOrEmpty(childrenHouseDTO.Name) ||
                childrenHouseDTO.Rating <= 0.0);
        }
    }
}
