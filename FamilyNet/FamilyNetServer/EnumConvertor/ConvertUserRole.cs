using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects.Enums;

namespace FamilyNetServer.EnumConvertor
{
    public class ConvertUserRole : IConvertUserRole
    {
        public UserRole ConvertFromString(string role)
        {
            UserRole result = UserRole.Undefined;

            if(!Enum.TryParse(role, out result))
            {
                result = UserRole.User;
            }

            return result;
            //if(role == "CharityMaker")
            //{
            //    return UserRole.CharityMaker;
            //}
            //else if(role =="Volunteer")
            //{
            //    return UserRole.Volunteer;
            //}
            //else
            //{
            //    return UserRole.Representative;
            //}
        }
    }
}
