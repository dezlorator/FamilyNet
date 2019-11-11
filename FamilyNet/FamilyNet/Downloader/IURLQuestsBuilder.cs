namespace FamilyNet.Downloader
{
    public interface IURLQuestsBuilder
    {
        string GetAllWithFilter(string api,
                       string forSearch,
                       string status);

        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
