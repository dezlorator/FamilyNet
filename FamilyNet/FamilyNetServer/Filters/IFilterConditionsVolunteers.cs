using FamilyNetServer.Models;
using System.Linq;

namespace FamilyNetServer.Filters
{
    public interface IFilterConditionsVolunteers
    {
        IQueryable<Volunteer> GetVolunteers(IQueryable<Volunteer> volunteers,
                                       string name, float rating,
                                       int age);
    }
}
