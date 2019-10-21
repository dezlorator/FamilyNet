using System.Linq;
using FamilyNetServer.Models;

namespace FamilyNetServer.Filters
{
    public class DonationsFilter : IDonationsFilter
    {
        public IQueryable<Donation> GetDonations(IQueryable<Donation> donations, int? orphanageID)
        {
            if (orphanageID != null && orphanageID > 0)
            {
                donations = donations.Where(d => d.OrphanageID == orphanageID);
            }

            return donations;
        }
    }
}
