using DataTransferObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IServerAvailabilitiesDownloader
    {
        Task<HttpStatusCode> CreatePostAsync(string url, AvailabilityDTO dto, ISession session);
    }
}
