using FamilyNet.Configuration;
using FamilyNet.Models.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace FamilyNet.Downloader
{
    public class URLVolunteersBuilder : IURLVolunteersBuilder
    {
        #region private fields

        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        #region ctor

        public URLVolunteersBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        #endregion

        public string GetAllWithFilter(string api,
                                       PersonSearchModel searchModel,
                                       int addressId)
        {
            var queryParams = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(searchModel.FullNameString))
            {
                queryParams.Add("name", searchModel.FullNameString);
            }

            if (searchModel.AgeNumber > 0)
            {
                queryParams.Add("age", searchModel.AgeNumber.ToString());
            }

            if (searchModel.RatingNumber > 0)
            {
                queryParams.Add("rating", searchModel.RatingNumber.ToString());
            }

            if (addressId > 0)
            {
                queryParams.Add("addressId", addressId.ToString());
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
