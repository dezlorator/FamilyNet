using System.Linq;
using FamilyNetServer.Models;

namespace FamilyNetServer.Filters
{
    public interface IFilterConditionsRepresentatives
    {
        IQueryable<Representative> GetRepresentatives(IQueryable<Representative> representatives,
                                       string name, float rating,
                                       int age);
    }
}
