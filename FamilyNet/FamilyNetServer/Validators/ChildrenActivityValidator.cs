using DataTransferObjects;
using System;

namespace FamilyNetServer.Validators
{
    public class ChildrenActivityValidator : IChildrenActivityValidator
    {
        public bool IsValid(ChildrenActivityDTO childrenActivityDTO)
        {
            if (String.IsNullOrEmpty(childrenActivityDTO.Name) ||
                String.IsNullOrEmpty(childrenActivityDTO.Description))
            {
                return false;
            }

            foreach (var a in childrenActivityDTO.Awards)
            {
                if (String.IsNullOrEmpty(a.Name) ||
                    String.IsNullOrEmpty(a.Description))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
