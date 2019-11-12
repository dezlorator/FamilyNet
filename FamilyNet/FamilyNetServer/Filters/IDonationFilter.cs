using FamilyNetServer.Models;
using System.Linq;

namespace FamilyNetServer.Filters
{
    public interface IDonationsFilter
    {
        IQueryable<Donation> GetDonations(IQueryable<Donation> donations,
                                          string filter, DonationStatus status, bool isRequest);
    }
}