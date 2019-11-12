namespace FamilyNet.Downloader
{
    public interface IURLDonationsBuilder
    {
        string GetAllWithFilter(string api,
                               string forSearch, 
                               string status = "Needed",
                               bool isRequest = true);

        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
