using FamilyNet.Configuration;
using FamilyNet.Models.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace FamilyNet.Downloader
{
    public class URLCategoriesBuilder : IURLCategoriesBuilder
    {
        #region private fields

        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        #region ctor

        public URLCategoriesBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        #endregion

        public string GetAll(string api)
        {
            var queryParams = new Dictionary<string, string>();

            return QueryHelpers.AddQueryString(_options.Value.ServerURL + api,
                                                queryParams);
        }

        public string GetById(string api, int id)
        {
            return _options.Value.ServerURL + api + "/" + id;
        }

        public string CreatePost(string api)
        {
            return _options.Value.ServerURL + api;
        }
    }
}
