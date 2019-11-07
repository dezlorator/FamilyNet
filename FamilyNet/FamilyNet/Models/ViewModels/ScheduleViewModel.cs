using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.ViewModels
{
    public class ScheduleViewModel
    {
        public IEnumerable<DayOfWeek> Days { get; set; }
        public IEnumerable<TimeSpan> Hours { get; set; }
        public IEnumerable<DateTime> Date { get; set; }
        public IDictionary<TimeSpan, IEnumerable<AvailabilityDTO>> Sorted { get; set; }
    }
}
