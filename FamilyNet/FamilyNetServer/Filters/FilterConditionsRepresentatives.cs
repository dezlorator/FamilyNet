﻿using System;
using System.Linq;
using FamilyNetServer.Models;
using FamilyNetServer.Filters.FilterParameters;

namespace FamilyNetServer.Filters
{
    public class FilterConditionsRepresentatives : IFilterConditionsRepresentatives
    {
        public IQueryable<Representative> GetRepresentatives(IQueryable<Representative> representatives,
                                             FilterParametersRepresentatives filter)
        {
            if (representatives == null || filter == null)
            {
                return representatives;
            }

            if (filter.ChildrenHouseID > 0)
            {
                representatives = representatives
                    .Where(c => c.OrphanageID == filter.ChildrenHouseID);
            }

            if (!String.IsNullOrEmpty(filter.Name))
            {
                representatives = representatives
                    .Where(c => c.FullName.ToString().ToUpper().Contains(filter.Name.ToUpper()));
            }

            if (filter.Rating > 0.001)
            {
                representatives = representatives
                    .Where(c => c.Rating <= filter.Rating);
            }

            if (filter.Age > 0)
            {
                var dayPerYear = 366;
                representatives = representatives.Where(c => (DateTime.Now - c.Birthday).Days >= filter.Age * dayPerYear);
            }

            if (filter.Rows > 0 && filter.Page > 0)
            {
                representatives = representatives
                    .Skip(filter.Rows * (filter.Page - 1))
                    .Take(filter.Rows);
            }

            return representatives;
        }
    }
}
