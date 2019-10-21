using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IAuthorizeCreater
    {
        Task<string> Login(string email, string password);
    }
}
