using DataTransferObjects;
using DataTransferObjects.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader.Interfaces
{
    public interface IFioDownloader
    {
        Task<FioDTO> GetByIdAsync(string url, ISession session);
    }
}