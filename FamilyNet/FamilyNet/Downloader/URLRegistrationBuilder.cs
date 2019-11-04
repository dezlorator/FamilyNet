using System;
using Microsoft.Extensions.Options;
using FamilyNet.Configuration;

namespace FamilyNet.Downloader
{
    public class URLRegistrationBuilder:IURLRegistrationBuilder
    {
        #region private fields

        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        #region ctor

        public URLRegistrationBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        #endregion
        public string Register(string api)
        {
            return _options.Value.ServerURL + api;
        }

    }
}
