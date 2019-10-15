using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IServerDataDownLoader<T> where T : class, new()
    {
        Task<List<T>> GetAllAsync(string url);
        Task<T> GetByIdAsync(string url);
        Task<HttpStatusCode> СreatetePostAsync(string url, T dto);
    }
}
