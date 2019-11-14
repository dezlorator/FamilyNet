using DataTransferObjects;
using FamilyNetServer.Enums;
using FamilyNetServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Filters
{
    public class FilterConditionPurchase : IFilterConditionPurchase
    {
        public IEnumerable<Purchase> GetFiltered(IEnumerable<Purchase> purchase,
           FilterParamentrsPurchaseDTO filter, string userId, out int count)
        {
            
            if (!String.IsNullOrEmpty(userId))
            {
                purchase = purchase.Where(o => o.UserId.ToString() == userId);
            }

            if (filter.Date > DateTime.MinValue)
            {
                purchase = purchase.Where(o => o.Date.Day == filter.Date.Month &&
                o.Date.Month == filter.Date.Day &&
                o.Date.Year == filter.Date.Year);
            }

            if (!String.IsNullOrEmpty(filter.CraftName))
            {               
                purchase = purchase.Where(o => o.AuctionLot.AuctionLotItem.Name.Contains(filter.CraftName));
            }


            Enum.TryParse<PurchaseSortState>(filter.Sort, out var order);

            switch (order)
            {
                case PurchaseSortState.DateAsc:
                    purchase = purchase.OrderBy(s => s.Date);
                    break;
                case PurchaseSortState.PaidAsc:
                    purchase = purchase.OrderBy(s => s.Paid);
                    break;
                case PurchaseSortState.PaidDesc:
                    purchase = purchase.OrderByDescending(s => s.Paid);
                    break;
                case PurchaseSortState.QuantityAsc:
                    purchase = purchase.OrderBy(s => s.Quantity);
                    break;
                case PurchaseSortState.QuantityDesc:
                    purchase = purchase.OrderByDescending(s => s.Quantity);
                    break;

                default:
                    purchase = purchase.OrderByDescending(s => s.Date);
                    break;
            }

            count = purchase.Count();

            if (filter.Rows > 0 & filter.Page > 0)
            {
                purchase = purchase.Skip((filter.Page - 1) * filter.Rows).Take(filter.Rows);
            }

            return purchase;
        }

  
    }
}
