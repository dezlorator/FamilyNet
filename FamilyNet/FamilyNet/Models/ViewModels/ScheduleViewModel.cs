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
        //public IEnumerable<AvailabilityDTO> Sunday { get; set; }
        //public IEnumerable<AvailabilityDTO> Monday { get; set; }
        //public IEnumerable<AvailabilityDTO> Tuesday { get; set; }
        //public IEnumerable<AvailabilityDTO> Wednesday { get; set; }
        //public IEnumerable<AvailabilityDTO> Thursday { get; set; }
        //public IEnumerable<AvailabilityDTO> Friday { get; set; }
        //public IEnumerable<AvailabilityDTO> Saturday { get; set; }

        //public IEnumerable<IGrouping<TimeSpan,AvailabilityDTO>> AvailabilityDTOList {get; set;}
    }
}
