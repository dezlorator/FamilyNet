using System;
using FamilyNet.Configuration;
using FamilyNet.Models.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace FamilyNet.Downloader
{
    public class URLCharityMakerBuilder : IURLCharityMakerBuilder
    {
        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        public URLCharityMakerBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        public string CreatePost(string api)
        {
            return _options.Value.ServerURL + api;
        }

        public string GetAllWithFilter(string api, PersonSearchModel searchModel, int charityMakerId)
        {
            var queryParams = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(searchModel.FullNameString))
            {
                queryParams.Add("name", searchModel.FullNameString);
            }

            if (searchModel.RatingNumber > 0)
            {
                queryParams.Add("rating", searchModel.RatingNumber.ToString());
            }

            if (charityMakerId > 0)
            {
                queryParams.Add("childrenHouseId", charityMakerId.ToString());
            }

            return QueryHelpers.AddQueryString(_options.Value.ServerURL + api,
                                                queryParams);
        }

        public string GetById(string api, int id)
        {
            return _options.Value.ServerURL + api + "/" + id;
        }
    }
}
