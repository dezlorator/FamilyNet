using FamilyNet.Models;
using FamilyNet.Models.ViewModels;

namespace FamilyNet.Downloader
{
    public interface IURLChildrenHouseBuilder
    {
        string GetAllWithFilter(string api,
                              OrphanageSearchModel searchModel,
                              SortStateOrphanages sortOrder);

        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
