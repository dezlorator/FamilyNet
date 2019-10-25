using FamilyNet.Models.ViewModels;

namespace FamilyNet.Downloader
{
    public interface IURLChildrenBuilder
    {
        string GetAllWithFilter(string api,
                                PersonSearchModel searchModel,
                                int orphanageId);

        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
