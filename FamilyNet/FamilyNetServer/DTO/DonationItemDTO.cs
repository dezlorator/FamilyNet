using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using FamilyNetServer.Enums;

namespace FamilyNetServer.DTO
{
    public class DonationItemDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public ICollection<string> Categories { get; set; }
        public IEnumerable<int> CategoriesID { get; set; }
    }
}
