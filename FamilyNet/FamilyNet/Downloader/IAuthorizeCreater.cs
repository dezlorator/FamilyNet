using DataTransferObjects;
using FamilyNet.HttpHandlers;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IAuthorizeCreater
    {
        Task<AuthenticationResult> Login(CredentialsDTO credentials);
    }
}
