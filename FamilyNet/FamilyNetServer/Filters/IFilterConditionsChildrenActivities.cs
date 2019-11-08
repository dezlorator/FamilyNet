using FamilyNetServer.Filters.FilterParameters;
using FamilyNetServer.Models;
using System.Linq;

namespace FamilyNetServer.Filters
{
    public interface IFilterConditionsChildrenActivities
    {
        IQueryable<ChildActivity> GetChildrenActivities(IQueryable<ChildActivity> activities,
                                            FilterParemetersChildrenActivities filter);
    }
}
