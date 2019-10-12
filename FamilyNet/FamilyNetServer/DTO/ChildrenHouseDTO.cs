﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.DTO
{
    public class ChildrenHouseDTO: AddressDTO
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int? AdressID { get; set; }

        public float Rating { get; set; }

        public IFormFile Avatar { get; set; }

        public int? LocationID { get; set; }

        public string PhotoPath { get; set; }

    }
}
