using FamilyNet.Configuration;
using FamilyNet.Models.ViewModels;
using Microsoft.Extensions.Options;
using System;
using System.Text;

namespace FamilyNet.Downloader
{
    public class URLChildrenBuilder
    {
        #region private fields

        private readonly IOptionsSnapshot<ServerURLSettings> _options;

        #endregion

        #region ctor

        public URLChildrenBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        #endregion

        public string GetAllWithFilter(string api,
                                       PersonSearchModel searchModel,
                                       int orphanageId)
        {
            var url = new StringBuilder(_options.Value.ServerURL);
            url.Append(api);
            url.Append("?");

            if (searchModel.AgeNumber > 0)
            {
                url.AppendFormat("age={0}&", searchModel.AgeNumber);
            }

            if (searchModel.RatingNumber > 0)
            {
                url.AppendFormat("rating={0}&", searchModel.RatingNumber);
            }

            if (!String.IsNullOrEmpty(searchModel.FullNameString))
            {
                url.AppendFormat("name={0}&", searchModel.FullNameString);
            }

            if (orphanageId > 0)
            {
                url.AppendFormat("childrenHouseId={0}&", orphanageId);
            }

            return url.ToString();
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