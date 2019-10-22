﻿using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IServerAddressDownloader
    {
        Task<IEnumerable<AddressDTO>> GetAllAsync(string url);
        Task<HttpResponseMessage> CreatePostAsync(string url, AddressDTO dto);
        Task<HttpResponseMessage> CreatePutAsync(string url, AddressDTO dto);
        Task<AddressDTO> GetByIdAsync(string url);
        Task<HttpStatusCode> DeleteAsync(string url);
    }
}
