namespace FamilyNet.Downloader
{
    interface IURLQuestsBuilder
    {
        string GetAllWithFilter(string api,
                       string forSearch);

        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
