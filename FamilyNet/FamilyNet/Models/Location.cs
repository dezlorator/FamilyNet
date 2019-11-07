using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FamilyNet.Models
{
    public class Location 
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
