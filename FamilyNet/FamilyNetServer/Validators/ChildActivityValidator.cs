using DataTransferObjects;
using System;

namespace FamilyNetServer.Validators
{
    public class ChildActivityValidator : IChildActivityValidator
    {
        public bool IsValid(ChildActivityDTO childrenActivityDTO)
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
