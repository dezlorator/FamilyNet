using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObjects;

namespace FamilyNetServer.Helpers
{
    public class ScheduleHelper : IScheduleHelper
    {
        public double AdjustDate(AvailabilityDTO availabilityDTO)
        {
            var daysInWeek = 7;

            var diff = (double)availabilityDTO.DayOfWeek - (double)DateTime.Now.DayOfWeek;

            if (diff < 0)
            {
                diff += daysInWeek;
            }

            if (diff == 0)
            {
                var timeDiff = availabilityDTO.StartTime < DateTime.Now;
                diff += (timeDiff) ? daysInWeek : 0;
            }

            return diff;
        }
    }
}
