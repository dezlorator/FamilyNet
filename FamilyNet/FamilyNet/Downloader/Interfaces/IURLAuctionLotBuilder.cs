using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IURLAuctionLotBuilder
    {
        string GetById(string api, int id);
        string SimpleQuery(string api);
    }
}
