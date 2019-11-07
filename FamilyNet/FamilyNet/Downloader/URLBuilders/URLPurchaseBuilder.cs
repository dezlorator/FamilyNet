using FamilyNet.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader.URLBuilders
{
    public class URLPurchaseBuilder : IURLPurchaseBuilder
    {
        #region private fields

        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        #region ctor

        public URLPurchaseBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        #endregion

        public string GetById(string api, int id)
        {
            return _options.Value.ServerURL + api + "/" + id;
        }

        public string SimpleQuery(string api)
        {
            return _options.Value.ServerURL + api;
        }
    }
}
