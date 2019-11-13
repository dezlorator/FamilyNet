using FamilyNet.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class URLAuctionLotBuilder : IURLAuctionLotBuilder
    {
        #region private fields

        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        #region ctor

        public URLAuctionLotBuilder(IOptionsSnapshot<ServerURLSettings> options)
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

        public string GetAllWithFilter(string api, string name,
            float priceStart, float priceEnd, string sort, int page, int rows)
        {
            var queryParams = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(name))
            {
                queryParams.Add("name", name);
            }

            if (priceStart > 0.0f)
            {
                queryParams.Add("priceStart", priceStart.ToString());
            }

            if (priceEnd > 0.0f)
            {
                queryParams.Add("priceEnd", priceEnd.ToString());
            }

            if (!String.IsNullOrEmpty(sort))
            {
                queryParams.Add("sort", sort);
            }

          
            if (rows > 0 && page > 0)
            {
                queryParams.Add("rows", rows.ToString());
                queryParams.Add("page", page.ToString());
            }

            return QueryHelpers.AddQueryString(_options.Value.ServerURL + api +"/approved",
                                                queryParams);
        }

        public string GetAllOrphanCrafts(string api, int orphanId, int page, int rows)
        {
            var queryParams = new Dictionary<string, string>();

            if (orphanId > 0)
            {
                queryParams.Add("orphanId", orphanId.ToString());
            }
         
            if (rows > 0 && page > 0)
            {
                queryParams.Add("rows", rows.ToString());
                queryParams.Add("page", page.ToString());
            }

            return QueryHelpers.AddQueryString(_options.Value.ServerURL + api,
                                                queryParams);
        }

        public string GetAllUnApproved(string api, int orphanId)
        {
            var queryParams = new Dictionary<string, string>();

            if (orphanId > 0)
            {
                queryParams.Add("orphanId", orphanId.ToString());
            }

            return QueryHelpers.AddQueryString(_options.Value.ServerURL + api + "/confirm",
                                                queryParams);
        }
    }
}
