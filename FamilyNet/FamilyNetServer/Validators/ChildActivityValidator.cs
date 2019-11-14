using DataTransferObjects;
using System;

namespace FamilyNetServer.Validators
{
    public class ChildActivityValidator : IValidator<ChildActivityDTO>
    {
        public bool IsValid(ChildActivityDTO childActivityDTO)
        {
            if (childActivityDTO == null)
            {
                return false;
            }

            if (childActivityDTO.ChildID <= 0)
            {
                return false;
            }

            if (String.IsNullOrEmpty(childActivityDTO.Name) ||
                String.IsNullOrEmpty(childActivityDTO.Description))
            {
                return false;
            }

            if (childActivityDTO.Awards != null)
            {
                foreach (var a in childActivityDTO.Awards)
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