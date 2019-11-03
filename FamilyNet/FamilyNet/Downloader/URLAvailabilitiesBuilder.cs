using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using FamilyNet.Configuration;
using FamilyNet.Models.ViewModels;

namespace FamilyNet.Downloader
{
    public class URLAvailabilitiesBuilder : IURLAvailabilitiesBuilder
    {
        #region private fields

        private IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        public URLAvailabilitiesBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        public string CreatePost(string api)
        {
            return _options.Value.ServerURL + api;
        }
    }
}
