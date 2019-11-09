using DataTransferObjects.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataTransferObjects
{
    public class AvailabilityDTO
    {
        public int ID { get; set; }
        //public int PersonID { get; set; }
        public PersonType Role { get; set; }

        [Required(ErrorMessage ="Please, select the day of week")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan FreeHours { get; set; }
        public bool IsReserved { get; set; }

    }
}
