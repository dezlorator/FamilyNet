using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Downloader
{
    public interface IURLPurchaseBuilder
    {
        string GetById(string api, int id);
        string SimpleQuery(string api);

        string GetAllFiltered(string api, FilterParamentrsPurchaseDTO filter);
    }
}
