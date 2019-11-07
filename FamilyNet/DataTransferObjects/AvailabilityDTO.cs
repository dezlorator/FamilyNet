﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataTransferObjects
{
    public class AvailabilityDTO
    {
        public int ID { get; set; }
        //public int VolunteerID { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; } //Time

        [DataType(DataType.Time)]
        public TimeSpan VolunteerHours { get; set; } //duration

        //TO-DO: add Date prop
    }
}
