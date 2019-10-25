using System;
using System.Linq;
using FamilyNetServer.Models;
using FamilyNetServer.Filters.FilterParameters;

namespace FamilyNetServer.Filters
{
    public class FilterConditionsVolunteers : IFilterConditionsVolunteers
    {
        public IQueryable<Volunteer> GetVolunteers(IQueryable<Volunteer> volunteers,
                                                   FilterParemetersVolunteers filter)
        {
            if (filter.AddressID > 0)
            {
                volunteers = volunteers.Where(c => c.AddressID == filter.AddressID);
            }

            if (!String.IsNullOrEmpty(filter.Name))
            {
                volunteers = volunteers.Where(c => c.FullName.ToString().Contains(filter.Name));
            }

            if (filter.Rating > 0.001)
            {
                volunteers = volunteers.Where(c => c.Rating > filter.Rating);
            }

            if (filter.Age > 0)
            {
                var daysPerYear = 366;
                volunteers = volunteers.Where(c => (DateTime.Now - c.Birthday).Days
                                                >= filter.Age * daysPerYear);
            }

            if (filter.Rows != 0 && filter.Page != 0)
            {
                volunteers = volunteers.Skip(filter.Rows * (filter.Page - 1))
                    .Take(filter.Rows);
            }

            return volunteers;
        }
    }
}
