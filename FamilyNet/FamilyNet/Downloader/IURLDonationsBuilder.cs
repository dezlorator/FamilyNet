using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IURLDonationsBuilder
    {
        string GetAllWithFilter(string api,
                               int orphanageId);

        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
