using System;
using System.Linq;
using FamilyNetServer.Models;

namespace FamilyNetServer.Filters
{
    public class FilterConditionsChildren : IFilterConditionsChildren
    {
        public IQueryable<Orphan> GetOrphans(IQueryable<Orphan> children,
                                             string name, float rating,
                                             int age)
        {
            if (!String.IsNullOrEmpty(name))
            {
                children = children.Where(c => c.FullName.ToString().Contains(name));
            }

            if (rating > 0.001)
            {
                children = children.Where(c => c.Rating > rating);
            }

            if (age > 0)
            {
                var dayPerYear = 366;
                children = children.Where(c => (DateTime.Now - c.Birthday).Days >= age * dayPerYear);
            }

            return children;
        }
    }
}