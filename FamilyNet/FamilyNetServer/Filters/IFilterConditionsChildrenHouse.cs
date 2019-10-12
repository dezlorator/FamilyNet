using FamilyNetServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Filters
{
    public interface IFilterConditionsChildrenHouse
    {
        IQueryable<Orphanage> GetFilteredChildrenHouses(IQueryable<Orphanage> childrenHouses,
                                       string name, float rating,
                                       string address);

        IQueryable<Orphanage> GetSortedChildrenHouses(IQueryable<Orphanage> childrenHouses,
                                      string sort);
    }
}
