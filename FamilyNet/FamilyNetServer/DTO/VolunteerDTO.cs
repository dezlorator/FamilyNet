using Microsoft.AspNetCore.Http;
using System;

namespace FamilyNetServer.DTO
{
    public class VolunteerDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public DateTime Birthday { get; set; }
        public float Rating { get; set; }
        public IFormFile Avatar { get; set; }
        public int EmailID { get; set; }
        public string PhotoPath { get; set; }
        public int? AddressID { get; set; }
    }
}
