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

            return quests.Where(d => (d.Donation.Orphanage.Name == filter
                                || d.Donation.Orphanage.Adress.City == filter
                                || d.Donation.Orphanage.Adress.Street == filter
                                || d.Donation.DonationItem.Name == filter
                                || d.Name == filter) && d.Status == status);
        }
    }
}
