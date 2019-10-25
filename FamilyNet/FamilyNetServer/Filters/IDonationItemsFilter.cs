using FamilyNetServer.Models;
using FamilyNetServer.Enums;
using System.Linq;
using System.Collections.Generic;

namespace FamilyNetServer.Filters
{
    public interface IDonationItemsFilter
    {
        IQueryable<DonationItem> GetDonationItems(IQueryable<DonationItem> donationItems,
                                  string Name, float minPrice, float maxPrice,
                                  string category);
    }
}