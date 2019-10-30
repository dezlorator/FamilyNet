namespace FamilyNet.Downloader
{
    public interface IURLDonationItemsBuilder
    {
        string GetAllWithFilter(string api,
                                string Name, float minPrice,
                                float maxPrice, string category);

        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
