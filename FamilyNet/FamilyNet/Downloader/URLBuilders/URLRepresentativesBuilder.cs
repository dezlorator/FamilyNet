using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using FamilyNet.Configuration;
using FamilyNet.Models.ViewModels;

namespace FamilyNet.Downloader
{
    public class URLRepresentativesBuilder : IURLRepresentativeBuilder
    {
        #region private fields

        private IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        public URLRepresentativesBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        public string CreatePost(string api)
        {
            return _options.Value.ServerURL + api;
        }

        public string GetAllWithFilter(string api, PersonSearchModel searchModel, int orphanageId)
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
    }
}