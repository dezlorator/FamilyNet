using FamilyNetServer.Models;
using System.Linq;

namespace FamilyNetServer.Filters
{
    public interface IFilterConditionsChildren
    {
        IQueryable<Orphan> GetOrphans(IQueryable<Orphan> children,
                                       string name, float rating,
                                       int age);
    }
}
