using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.DTO
{
    public class LocationDTO
    {
        public int ID { get; set; }

        public float? MapCoordX { get; set; }

        public float? MapCoordY { get; set; }

        public string ChildrenHouseName { get; set; }
    }
}
