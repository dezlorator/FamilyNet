using FamilyNet.Configuration;
using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class URLChildrenHouseBuilder : IURLChildrenHouseBuilder
    {
        #region private fields

        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        #region ctor

        public URLChildrenHouseBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        #endregion

        public string GetAllWithFilter(string api,
                                       OrphanageSearchModel searchModel,
                                       SortStateOrphanages sortOrder)
        {
            var queryParams = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(searchModel.Name))
            {
                queryParams.Add("name", searchModel.Name);
            }

            if (searchModel.Rating > 0)
            {
                queryParams.Add("rating", searchModel.Rating.ToString());
            }

            if (!String.IsNullOrEmpty(searchModel.Address))
            {
                queryParams.Add("address", searchModel.Address.ToString());
            }

            
            queryParams.Add("sort", sortOrder.ToString());
            

            if (searchModel.RowsCount > 0 && searchModel.Page > 0)
            {
                queryParams.Add("rowsPerPage", searchModel.RowsCount.ToString());
                queryParams.Add("page", searchModel.Page.ToString());
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
