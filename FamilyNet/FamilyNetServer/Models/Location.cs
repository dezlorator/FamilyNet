using FamilyNetServer.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models
{
    public class Location : IEntity
    {
        public float? MapCoordX { get; set; }

        public float? MapCoordY { get; set; }
        public int ID { get; set; }

        public virtual void CopyState(Location sender)
        {
            MapCoordX = sender.MapCoordX;
            MapCoordY = sender.MapCoordY;
        }

        [BindNever]
        public bool IsDeleted { get; set; } = false;
    }
}
