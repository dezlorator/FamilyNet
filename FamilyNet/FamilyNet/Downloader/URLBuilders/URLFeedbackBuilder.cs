using FamilyNet.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader.URLBuilders
{
    public class URLFeedbackBuilder : IURLFeedbackBuilder
    {
        #region private 
        private readonly IOptionsSnapshot<ServerURLSettings> _options;
        #endregion

        public URLFeedbackBuilder(IOptionsSnapshot<ServerURLSettings> options)
        {
            _options = options;
        }

        public string CreatePost(string api)
        {
            return _options.Value.ServerURL + api;
        }

        public string GetByDonationId(string api, int donationId)
        {
            return _options.Value.ServerURL + api + "?donationId=" + donationId;
        }

        public string GetById(string api, int id)
        {
            return _options.Value.ServerURL + api + "/" + id;
        }
    }
}
