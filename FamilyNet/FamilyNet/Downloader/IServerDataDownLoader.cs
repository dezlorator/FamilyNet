using System.Collections.Generic;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IServerDataDownLoader<T> where T : class, new()
    {
        Task<List<T>> GetAll(string url);
        Task<T> GetById(string url);
    }
}
