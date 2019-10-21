using System;
using System.Linq;
using FamilyNetServer.Models;

namespace FamilyNetServer.Filters
{
    public class FilterConditionsRepresentatives : IFilterConditionsRepresentatives
    {
        public IQueryable<Representative> GetRepresentatives(IQueryable<Representative> representatives,
                                             string name, float rating,
                                             int age)
        {
            if (!String.IsNullOrEmpty(name))
            {
                representatives = representatives.Where(c => c.FullName.ToString().Contains(name));
            }

            if (rating > 0.001)
            {
                representatives = representatives.Where(c => c.Rating > rating);
            }

            if (age > 0)
            {
                var dayPerYear = 366;
                representatives = representatives.Where(c => (DateTime.Now - c.Birthday).Days >= age * dayPerYear);
            }

            return representatives;
        }
    }
}
