using FamilyNet.Configuration;
using FamilyNet.Models.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace FamilyNet.Downloader
{
    public class URLDonationItemsBuilder : IURLDonationItemsBuilder
    {
        #region private fields

        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        #region ctor

        public URLDonationItemsBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        #endregion

        public string GetAllWithFilter(string api,
                                       string name, float minPrice,
                                       float maxPrice, string category)
        {
            var queryParams = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(name)) 
            {
                queryParams.Add("name", name);
            }

            if (minPrice > 0) 
            {
                queryParams.Add("minPrice", minPrice.ToString());
            }

            if (maxPrice > 0) 
            {
                queryParams.Add("maxPrice", maxPrice.ToString());
            }

            if (!String.IsNullOrEmpty(category))
            {
                queryParams.Add("category", category);
            }

            return QueryHelpers.AddQueryString(_options.Value.ServerURL + api,
                                                queryParams);
        }

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
