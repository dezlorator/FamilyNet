using FamilyNetServer.Models;
using System.Linq;

namespace FamilyNetServer.Filters
{
    public interface IQuestsFilter
    {
        IQueryable<Quest> GetQuests(IQueryable<Quest> quests,
                                          string filter);
    }
}
