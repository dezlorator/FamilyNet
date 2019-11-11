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
        public IEnumerable<Purchase> GetFiltered(IQueryable<Purchase> purchase,
           FilterParamentrsPurchaseDTO filter, out int count)
        {

            if (filter.PaidTo > 0.0)
            {
                purchase = purchase.Where(o => o.Paid < filter.PaidTo);
            }

            if (filter.PaidFrom > 0.0)
            {
                purchase = purchase.Where(o => o.Paid > filter.PaidFrom);
            }

            if (filter.QuantityFrom > 0.0)
            {
                purchase = purchase.Where(o => o.Quantity > filter.QuantityFrom);
            }

            if (filter.QuantityTo > 0.0)
            {
                purchase = purchase.Where(o => o.Quantity < filter.QuantityTo);
            }

            if (!String.IsNullOrEmpty(filter.UserId))
            {
                purchase = purchase.Where(o => o.UserId.ToString() == filter.UserId);
            }

            if (filter.Date > DateTime.MinValue)
            {
                purchase = purchase.Where(o => o.Date.Day == filter.Date.Month &&
                o.Date.Month == filter.Date.Day &&
                o.Date.Year == filter.Date.Year);
            }

            if (filter.CraftId > 0)
            {
                purchase = purchase.Where(o => o.AuctionLotId == filter.CraftId);
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
