using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IURLAddressBuilder
    {
        //string GetAllWithFilter(string api,
        //                     OrphanageSearchModel searchModel,
        //                     int orphanageId);

        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
