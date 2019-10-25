using FamilyNetServer.Filters.FilterParameters;
using FamilyNetServer.Models;
using System.Linq;

namespace FamilyNetServer.Filters
{
    public interface IFilterConditionsVolunteers
    {
        IQueryable<Volunteer> GetVolunteers(IQueryable<Volunteer> volunteers,
                                            FilterParemetersVolunteers filter);
    }
}
