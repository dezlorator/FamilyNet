using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Validators
{
    public class AvailabilityValidator : IValidator<AvailabilityDTO>
    {
        public bool IsValid(AvailabilityDTO availabilityDTO)
        {
            return true;
        }

    }
}
