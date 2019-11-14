using DataTransferObjects.Enums;
using FamilyNet.Configuration;
using FamilyNet.Downloader.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader.URLBuilders
{
    public class URLFioBuilder : IURLFioBuilder
    {
        #region private 
        private readonly IOptionsSnapshot<ServerURLSettings> _options;
        #endregion

        public URLFioBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }
        public string GetById(string api, int id, UserRole role)
        {
            return _options.Value.ServerURL + api + "/" + id + "?role=" + role;
        }
    }
}
