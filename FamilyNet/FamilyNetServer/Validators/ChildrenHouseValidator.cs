using FamilyNetServer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
