using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.DTO;

namespace FamilyNetServer.Validators
{
    public interface ICharityMakerValidator
    {
        bool IsValid(CharityMakerDTO charityMakerDTO);
    }
}
