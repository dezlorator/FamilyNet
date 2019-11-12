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

        string GetAllWithFilter(string api, string name,
            float priceStart, float priceEnd, string sort,
            int page, int rows);

        string GetAllOrphanCrafts(string api, int orphanId,
            int page, int rows);

        string GetAllUnApproved(string api, int orphanId);
    }
}
