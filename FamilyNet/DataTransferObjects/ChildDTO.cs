using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObjects
{
    public class ChildDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }
        public float Rating { get; set; }
        public IFormFile Avatar { get; set; }
        public int ChildrenHouseID { get; set; }

        [Display(Name = "Детский дом")]
        public string ChildrenHouseName { get; set; }
        public int EmailID { get; set; }
        public string PhotoPath { get; set; }
    }
}
