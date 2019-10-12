using FamilyNetServer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Validators
{
    public class ChildrenHouseValidator : IChildrenHouseValidator
    {
        public bool IsValid(ChildrenHouseDTO childrenHouseDTO)
        {
            return !(String.IsNullOrEmpty(childrenHouseDTO.Country) ||
                String.IsNullOrEmpty(childrenHouseDTO.Region) ||
                String.IsNullOrEmpty(childrenHouseDTO.City) ||
                String.IsNullOrEmpty(childrenHouseDTO.Street) ||
                String.IsNullOrEmpty(childrenHouseDTO.House) ||
                childrenHouseDTO.Rating <= 0.0);
        }
    }
}
