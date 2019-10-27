using DataTransferObjects;
using System;

namespace FamilyNetServer.Validators
{
    public class ChildValidator : IChildValidator
    {
        public bool IsValid(ChildDTO childDTO)
        {
            if (childDTO == null)
            {
                return false;
            }

            return DateTimeIsPresent(childDTO.Birthday) &&
                   NameIsPresent(childDTO.Name) &&
                   SurnameIsPresent(childDTO.Surname) &&
                   PatronymicIsPresent(childDTO.Patronymic) &&
                   ChildrenHouseIsPresent(childDTO.ChildrenHouseID);
        }

        private bool DateTimeIsPresent(DateTime birthday)
        {
            return birthday >= DateTime.MinValue;
        }

        private bool NameIsPresent(string name)
        {
            return !String.IsNullOrEmpty(name);
        }

        private bool SurnameIsPresent(string name)
        {
            return !String.IsNullOrEmpty(name);
        }

        private bool PatronymicIsPresent(string patronymic)
        {
            return !String.IsNullOrEmpty(patronymic);
        }

        private bool ChildrenHouseIsPresent(int childrenHouseId)
        {
            return childrenHouseId > 0;
        }
    }
}
