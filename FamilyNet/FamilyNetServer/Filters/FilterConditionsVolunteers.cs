using System;
using System.Linq;
using FamilyNetServer.Models;

namespace FamilyNetServer.Filters
{
    public class FilterConditionsVolunteers : IFilterConditionsVolunteers
    {
        public IQueryable<Volunteer> GetVolunteers(IQueryable<Volunteer> volunteers,
                                     string name, float rating,
                                     int age)
        {
            if (!String.IsNullOrEmpty(name))
            {
                volunteers = volunteers.Where(c => c.FullName.ToString().Contains(name));
            }

            if (rating > 0.001)
            {
                volunteers = volunteers.Where(c => c.Rating > rating);
            }

            if (age > 0)
            {
                var dayPerYear = 366;
                volunteers = volunteers.Where(c => (DateTime.Now - c.Birthday).Days >= age * dayPerYear);
            }

            return volunteers;
        }
    }
}
