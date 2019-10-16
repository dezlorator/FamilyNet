using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Filters.FilterParameters
{
    public class FilterParametersPerson
    {
        public int ChildrenHouseID { get; set; }
        public string Name { get; set; }
        public float Rating { get; set; }
        public int Rows { get; set; }
        public int Page { get; set; }
        public int Age { get; set; }
    }
}
