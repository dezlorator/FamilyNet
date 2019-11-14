using FamilyNetServer.Models;
using DataTransferObjects;

namespace FamilyNetServer.Validators
{
    public interface IAvailabilityValidator
    {
        bool IsValid(AvailabilityDTO availabilityDTO);
        bool IsOverlaping(AvailabilityDTO dto, Availability entity);
        bool IsCorrectStartTime(AvailabilityDTO dto);
        bool IsNotReserved(AvailabilityDTO dto);
        bool IsFreeTimeEnough(AvailabilityDTO dto);
    }
}
