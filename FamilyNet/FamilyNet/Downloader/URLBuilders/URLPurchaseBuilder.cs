using DataTransferObjects;
using FamilyNet.Configuration;
using Microsoft.AspNetCore.WebUtilities;
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

        public string GetAllFiltered(string api, FilterParamentrsPurchaseDTO filter)
        {
            var queryParams = new Dictionary<string, string>();

           

            if (!String.IsNullOrEmpty(filter.Email))
            {
                queryParams.Add("Email", filter.Email);
            }

            if (filter.Date > DateTime.MinValue)
            {
                queryParams.Add("Date", filter.Date.ToString());
            }

            if (!String.IsNullOrEmpty(filter.CraftName))
            {
                queryParams.Add("CraftName", filter.CraftName);
            }

            if (!String.IsNullOrEmpty(filter.Sort))
            {
                queryParams.Add("Sort", filter.Sort);
            }

            if (filter.Rows > 0 && filter.Page > 0)
            {
                queryParams.Add("Rows", filter.Rows.ToString());
                queryParams.Add("Page", filter.Page.ToString());
            }

            return QueryHelpers.AddQueryString(_options.Value.ServerURL + api ,
                                                queryParams);
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
