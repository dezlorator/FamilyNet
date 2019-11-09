using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.Models;

namespace FamilyNetServer.Filters
{
    public class QuestsFilter : IQuestsFilter
    {
        public IQueryable<Quest> GetQuests(IQueryable<Quest> quests, string filter, QuestStatus status)
        {
            if (filter == null)
            {
                return quests.Where(q => q.Status == status);
            }

            return quests.Where(d => (d.Donation.Orphanage.Name.Contains(filter)
                                || d.Donation.Orphanage.Adress.City.Contains(filter)
                                || d.Donation.Orphanage.Adress.Street.Contains(filter)
                                || d.Donation.DonationItem.Name.Contains(filter)
                                || d.Name.Contains(filter)) && d.Status == status);
        }
    }
}
