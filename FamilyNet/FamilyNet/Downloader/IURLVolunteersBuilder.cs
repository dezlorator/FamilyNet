using FamilyNet.Models.ViewModels;

namespace FamilyNet.Downloader
{
    public interface IURLVolunteersBuilder
    {
        string GetAllWithFilter(string api,
                                PersonSearchModel searchModel,
                                int adressId);

        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
