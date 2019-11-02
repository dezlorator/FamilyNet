using System.Linq;
using System.Collections.Generic;
using FamilyNetServer.Models;

namespace FamilyNetServer.Filters
{
    public class DonationsFilter : IDonationsFilter
    {
        public IQueryable<Donation> GetDonations(IQueryable<Donation> donations, string filter)
        {
            if (filter == null)
            {
                return donations;
            }

            return donations.Where(d => d.Orphanage.Name == filter
                                || d.Orphanage.Adress.City == filter
                                || d.Orphanage.Adress.Street == filter
                                || d.DonationItem.Name == filter
                                || d.DonationItem.TypeBaseItem
                                                 .Select(i => i.Type.Name)
                                                 .Contains(filter));
        }
    }
}
