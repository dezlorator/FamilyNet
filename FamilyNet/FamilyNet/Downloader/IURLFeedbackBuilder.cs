using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IURLFeedbackBuilder
    {
        string GetByDonationId(string api, int donationId);
        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
