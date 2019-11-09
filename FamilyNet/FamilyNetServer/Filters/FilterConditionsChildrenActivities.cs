using System;
using System.Linq;
using FamilyNetServer.Models;
using FamilyNetServer.Filters.FilterParameters;

namespace FamilyNetServer.Filters
{
    public class FilterConditionsChildrenActivities : IFilterConditionsChildrenActivities
    {
        public IQueryable<ChildActivity> GetChildrenActivities(IQueryable<ChildActivity> activities,
                                                   FilterParemetersChildrenActivities filter)
        {
            if (filter.ChildID > 0)
            {
                activities = activities.Where(a => a.Child.ID == filter.ChildID);
            }

            if (!String.IsNullOrEmpty(filter.Name))
            {
                activities = activities.Where(a => a.Name.ToUpper().Contains(filter.Name.ToUpper()));
            }

            if (filter.Rows != 0 && filter.Page != 0)
            {
                activities = activities.Skip(filter.Rows * (filter.Page - 1))
                    .Take(filter.Rows);
            }

            return activities;
        }
    }
}