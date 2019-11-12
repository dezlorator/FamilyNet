using System.Linq;
using System.Collections.Generic;
using FamilyNetServer.Models;

namespace FamilyNetServer.Filters
{
    public class DonationsFilter : IDonationsFilter
    {
        public IQueryable<Donation> GetDonations(IQueryable<Donation> donations, string filter, DonationStatus status, bool isRequest)
        {
            if (filter == null)
            {
                return donations;
            }

            return donations.Where(d => d.Status == status && d.IsRequest == isRequest
                                && d.Orphanage.Name.Contains(filter)
                                || d.Orphanage.Adress.City.Contains(filter)
                                || d.Orphanage.Adress.Street.Contains(filter)
                                || d.DonationItem.Name.Contains(filter)
                                || d.DonationItem.TypeBaseItem
                                                 .Select(i => i.Type.Name)
                                                 .Contains(filter));
        }
    }
}
