using DataTransferObjects;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IServerAddressDownloader
    {
        Task<IEnumerable<AddressDTO>> GetAllAsync(string url, ISession session);
        Task<HttpResponseMessage> CreatePostAsync(string url, AddressDTO dto, ISession session);
        Task<HttpResponseMessage> CreatePutAsync(string url, AddressDTO dto, ISession session);
        Task<AddressDTO> GetByIdAsync(string url, ISession session);
        Task<HttpStatusCode> DeleteAsync(string url, ISession session);
    }
}
