using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataTransferObjects
{
    public class QuestDTO
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }

        public string DonationName { get; set; }

        public string DonationDescription { get; set; }

        public string OrphanageName { get; set; }

        public int? VolunteerID { get; set; }

        public int? DonationID { get; set; }

        public int? OrphanageID { get; set; }

        public int? CharityMakerID { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }
    }
}