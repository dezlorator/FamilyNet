using FamilyNet.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace FamilyNet.Downloader
{
    public class URLQuestsBuilder : IURLQuestsBuilder
    {
        #region private fields

        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        #region ctor

        public URLQuestsBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        #endregion

        public string GetAllWithFilter(string api,
                                       string forSearch)
        {
            var queryParams = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(forSearch))
            {
                queryParams.Add("forSearch", forSearch);
            }

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
