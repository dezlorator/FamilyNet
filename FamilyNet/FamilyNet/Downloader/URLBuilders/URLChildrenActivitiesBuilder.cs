using FamilyNet.Configuration;
using FamilyNet.Models.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;


namespace FamilyNet.Downloader
{
    public class URLChildrenActivitiesBuilder : IURLChildrenActivitesBuilder
    {
        #region private fields

        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        #region ctor

        public URLChildrenActivitiesBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        #endregion

        public string GetAllWithFilter(string api,
                                       ChildActivitySearchModel model)
        {
            var queryParams = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(model.Name))
            {
                queryParams.Add("name", model.Name);
            }

            if (model.ChildID > 0)
            {
                queryParams.Add("childId", model.ChildID.ToString());
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