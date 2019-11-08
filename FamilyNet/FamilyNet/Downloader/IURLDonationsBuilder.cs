namespace FamilyNet.Downloader
{
    public interface IURLDonationsBuilder
    {
        string GetAllWithFilter(string api,
                               int orphanageId);

        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
