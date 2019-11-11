using DataTransferObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.EnumConvertor
{
    public interface IConvertUserRole
    {
        UserRole ConvertFromString(string role);
    }
}
