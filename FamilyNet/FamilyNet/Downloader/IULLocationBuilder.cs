namespace FamilyNet.Downloader
{
    public interface IURLLocationBuilder
    {
        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
