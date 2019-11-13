using DataTransferObjects;
using FamilyNetServer.Enums;
using FamilyNetServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Filters
{
    public interface IFilterConditionPurchase
    {
        IEnumerable<Purchase> GetFiltered(IEnumerable<Purchase> purchase,
           FilterParamentrsPurchaseDTO filter, string userId, out int count);
       
    }
}
