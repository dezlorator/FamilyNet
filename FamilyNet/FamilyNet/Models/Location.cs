using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models
{
    public class Location:IEntity
    {
        public float? MapCoordX { get; set; }

        public float? MapCoordY { get; set; }
        public int ID { get; set; }
    }
}
