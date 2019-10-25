using System.Linq;
using FamilyNetServer.Models;
using FamilyNetServer.Filters.FilterParameters;

namespace FamilyNetServer.Filters
{
    public interface IFilterConditionsRepresentatives
    {
        IQueryable<Representative> GetRepresentatives(IQueryable<Representative> representatives,
                                      FilterParametersRepresentatives filter);
    }
}
