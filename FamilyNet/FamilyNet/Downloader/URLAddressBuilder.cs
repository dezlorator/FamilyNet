using FamilyNet.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public class URLAddressBuilder : IURLAddressBuilder
    {
        #region private fields

        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        #region ctor

        public URLAddressBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        #endregion

        //public string GetAllWithFilter(string api,
        //                               OrphanageSearchModel searchModel,
        //                               int orphanageId)
        //{
        //    var queryParams = new Dictionary<string, string>();

        //    if (!String.IsNullOrEmpty(searchModel.NameString))
        //    {
        //        queryParams.Add("name", searchModel.NameString);
        //    }

        //    if (searchModel.RatingNumber > 0)
        //    {
        //        queryParams.Add("rate", searchModel.RatingNumber.ToString());
        //    }

        //    if (searchModel.RatingNumber > 0)
        //    {
        //        queryParams.Add("rating", searchModel.RatingNumber.ToString());
        //    }


        //    return QueryHelpers.AddQueryString(_options.Value.ServerURL + api,
        //                                        queryParams);
        //}

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
