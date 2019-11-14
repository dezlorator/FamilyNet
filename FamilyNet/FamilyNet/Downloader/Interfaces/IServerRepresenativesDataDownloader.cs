using DataTransferObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader.Interfaces
{
    public interface IServerRepresenativesDataDownloader
    {
        Task<List<RepresentativeDTO>> GetByChildrenHouseIdAsync(string url, ISession session);
    }
}