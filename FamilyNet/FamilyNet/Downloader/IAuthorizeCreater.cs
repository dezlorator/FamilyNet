using DataTransferObjects;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IAuthorizeCreater
    {
        Task<string> Login(CredentialsDTO credentials);
    }
}
