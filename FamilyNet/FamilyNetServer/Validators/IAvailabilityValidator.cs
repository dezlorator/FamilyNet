using DataTransferObjects;
using FamilyNetServer.Models;

namespace FamilyNetServer.Validators
{
    public interface IAvailabilityValidator
    {
        bool IsValid(AvailabilityDTO availabilityDTO);
        bool IsOverlaping(AvailabilityDTO dto, Availability entity);
    }
}
