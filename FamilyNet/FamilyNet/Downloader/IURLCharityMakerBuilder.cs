using FamilyNet.Models.ViewModels;

namespace FamilyNet.Downloader
{
    public interface IURLCharityMakerBuilder
    {
        string GetAllWithFilter(string api,
                        PersonSearchModel searchModel,
                        int charityMakerId);

        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
