using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.Models;

namespace FamilyNetServer.Filters
{
    public class DonationItemsFilter : IDonationItemsFilter
    {
        public IQueryable<DonationItem> GetDonationItems(IQueryable<DonationItem> donationItems,
                                  string Name, float minPrice, float maxPrice,
                                  string category)
        {
            if (Name != String.Empty)
            {
                donationItems = donationItems.Where(i => i.Name == Name);
            }

            if (minPrice > 0)
            {
                donationItems = donationItems.Where(i => i.Price >= minPrice);
            }

            if (maxPrice > 0)
            {
                donationItems = donationItems.Where(i => i.Price <= maxPrice);
            }

            if (category != String.Empty)
            {
                donationItems = donationItems.Where(i => i.TypeBaseItem
                                                          .Select(d => d.Type.Name)
                                                          .Contains(category)
                                                          );
            }

            return donationItems;
        }
    }
}