using DataTransferObjects;

namespace FamilyNetServer.Helpers
{
    public interface IScheduleHelper
    {
        double AdjustDate(AvailabilityDTO availabilityDTO);
    }
}
