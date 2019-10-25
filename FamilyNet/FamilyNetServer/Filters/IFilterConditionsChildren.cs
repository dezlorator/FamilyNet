using FamilyNetServer.Filters.FilterParameters;
using FamilyNetServer.Models;
using System.Linq;

namespace FamilyNetServer.Filters
{
    public interface IFilterConditionsChildren
    {
        IQueryable<Orphan> GetOrphans(IQueryable<Orphan> children,
                                      FilterParemetersChildren filter);
    }
}
