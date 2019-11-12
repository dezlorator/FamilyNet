using FamilyNet.Models.ViewModels;

namespace FamilyNet.Downloader
{
    public interface IURLRepresentativeBuilder
    {
        string GetAllWithFilter(string api,
                                PersonSearchModel searchModel,
                                int orphanageId);

        string GetById(string api, int id);
        string GetByChildrenHouseId(string api, int childrenHouseId);
        string CreatePost(string api);
    }
}
