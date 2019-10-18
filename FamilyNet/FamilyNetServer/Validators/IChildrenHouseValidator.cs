using FamilyNetServer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Validators
{
    public interface IChildrenHouseValidator
    {
        bool IsValid(ChildrenHouseDTO childrenHouseDTO);
    }
}
