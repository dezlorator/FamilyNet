using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IURLChildrenHouseBuilder
    {
        string GetAllWithFilter(string api,
                              OrphanageSearchModel searchModel,
                              SortStateOrphanages sortOrder);

        string GetById(string api, int id);
        string CreatePost(string api);
    }
}
