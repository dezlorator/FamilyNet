using System;
using System.Linq;
using FamilyNetServer.Filters.FilterParameters;
using FamilyNetServer.Models;
using Microsoft.Extensions.Logging;

namespace FamilyNetServer.Filters
{
    public class FilterConditionsChildren : IFilterConditionsChildren
    {
        private readonly ILogger<FilterConditionsChildren> _logger;

        public FilterConditionsChildren(ILogger<FilterConditionsChildren> logger)
        {
            _logger = logger;
        }

        public IQueryable<Orphan> GetOrphans(IQueryable<Orphan> children,
                                             FilterParemetersChildren filter)
        {
            if (children == null || filter == null)
            {
                _logger.LogInformation("children or filter are null");
                return children;
            }

            if (filter.ChildrenHouseID > 0)
            {
                _logger.LogInformation("filter by childrenhouse " + filter.ChildrenHouseID);
                children = children.Where(c => c.OrphanageID == filter.ChildrenHouseID);
            }

            if (!String.IsNullOrEmpty(filter.Name))
            {
                _logger.LogInformation("filter by name " + filter.Name);
                children = children.Where(c => c.FullName.ToString().Contains(filter.Name));
            }

            if (filter.Rating > 0.001)
            {
                _logger.LogInformation("filter by rating " + filter.Rating);
                children = children.Where(c => c.Rating > filter.Rating);
            }

            if (filter.Age > 0)
            {
                _logger.LogInformation("filter by age " + filter.Age);
                var daysPerYear = 366;
                children = children.Where(c => (DateTime.Now - c.Birthday).Days
                                                >= filter.Age * daysPerYear);
            }

            if (filter.Rows > 0 && filter.Page > 0)
            {
                _logger.LogInformation("paging children: rows " + filter.Rows
                    + " page: " + filter.Page);
                children = children.Skip(filter.Rows * (filter.Page - 1))
                    .Take(filter.Rows);
            }

            return children;
        }
    }
}