using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.DTO
{
    public class CharityMakerDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public DateTime Birthday { get; set; }
        public float Rating { get; set; }
        public IFormFile Avatar { get; set; }
        public int CharityMakerID { get; set; }
        public int EmailID { get; set; }
        public string PhotoPath { get; set; }
    }
}
