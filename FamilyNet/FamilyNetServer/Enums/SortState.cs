using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Filters
{
    public enum SortState
    {
        Default = 0,
        NameAsc,
        NameDesc,
        AddressAsc,
        AddressDesc,
        RatingAsc,
        RatingDesc
    }
}
