using FamilyNetServer.Models;
using FamilyNetServer.Enums;
using System.Linq;

namespace FamilyNetServer.Filters
{
    public interface IDonationsFilter
    {
        IQueryable<Donation> GetDonations(IQueryable<Donation> donations,
                                          int? orphanageID);
    }
}