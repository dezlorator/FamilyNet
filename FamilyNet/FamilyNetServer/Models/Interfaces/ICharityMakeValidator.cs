using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.DTO;

namespace FamilyNetServer.Models.Interfaces
{
    public interface ICharityMakeValidator
    {
        bool IsValid(CharityMakerDTO charityMakerDTO);
    }
}
