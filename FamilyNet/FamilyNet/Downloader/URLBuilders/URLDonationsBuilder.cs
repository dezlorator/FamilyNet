using FamilyNet.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace FamilyNet.Downloader
{
    public class URLDonationsBuilder: IURLDonationsBuilder
    {
        #region private fields

        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        #region ctor

        public URLDonationsBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        #endregion

        public string GetAllWithFilter(string api,
                                       int orphanageId)
        {
            var queryParams = new Dictionary<string, string>();

            if (orphanageId > 0)
            {
                queryParams.Add("childrenHouseId", orphanageId.ToString());
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
