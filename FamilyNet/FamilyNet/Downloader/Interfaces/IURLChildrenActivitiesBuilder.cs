using FamilyNet.Models.ViewModels;

namespace FamilyNet.Downloader
{
    public interface IURLChildrenActivitesBuilder
    {
        string GetAllWithFilter(string api,
                                ChildActivitySearchModel model);

        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
