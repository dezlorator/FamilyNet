using DataTransferObjects;
using System;

namespace FamilyNetServer.Validators
{
    public class ChildActivityValidator : IValidator<ChildActivityDTO>
    {
        public bool IsValid(ChildActivityDTO childrenActivityDTO)
        {
            if (childrenActivityDTO.ChildID <= 0)
            {
                return false;
            }

            if (String.IsNullOrEmpty(childrenActivityDTO.Name) ||
                String.IsNullOrEmpty(childrenActivityDTO.Description))
            {
                return false;
            }

            if (childrenActivityDTO.Awards != null)
            {
                foreach (var a in childrenActivityDTO.Awards)
                {
                    if (String.IsNullOrEmpty(a.Name) ||
                        String.IsNullOrEmpty(a.Description))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}