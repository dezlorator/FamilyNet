using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using FamilyNet.Configuration;
using FamilyNet.Models.ViewModels;
namespace FamilyNet.Downloader
{
    public class URLRolesBuilder:IURLRolesBuilder
    {

        #region private fields

        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        #region ctor

        public URLRolesBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        #endregion

        public string GetAll(string api)
        {
            return _options.Value.ServerURL + api;
        }

        public string Delete(string api, string id)
        {
            return _options.Value.ServerURL + api + "/" + id;
        }

        public string CreatePost(string api)
        {
            return _options.Value.ServerURL + api;
        }
    }
}
