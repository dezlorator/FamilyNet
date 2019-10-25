using System;
using System.Linq;
using FamilyNetServer.Filters.FilterParameters;
using FamilyNetServer.Models;

namespace FamilyNetServer.Filters
{
    public class FilterConditionsChildren : IFilterConditionsChildren
    {
        public IQueryable<Orphan> GetOrphans(IQueryable<Orphan> children,
                                             FilterParemetersChildren filter)
        {
            if (filter.ChildrenHouseID > 0)
            {
                children = children.Where(c => c.OrphanageID == filter.ChildrenHouseID);
            }

            if (!String.IsNullOrEmpty(filter.Name))
            {
                children = children.Where(c => c.FullName.ToString().Contains(filter.Name));
            }

            if (filter.Rating > 0.001)
            {
                children = children.Where(c => c.Rating > filter.Rating);
            }

            if (filter.Age > 0)
            {
                var daysPerYear = 366;
                children = children.Where(c => (DateTime.Now - c.Birthday).Days
                                                >= filter.Age * daysPerYear);
            }

            if (filter.Rows != 0 && filter.Page != 0)
            {
                children = children.Skip(filter.Rows * (filter.Page - 1))
                    .Take(filter.Rows);
            }

            return children;
        }
    }
}