using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models.ViewModels
{
    public class PersonSearchModel
    {
        public string FullNameString { get; set; }

        public float RatingNumber { get; set; }
        public int AgeNumber { get; set; }
    }
}
