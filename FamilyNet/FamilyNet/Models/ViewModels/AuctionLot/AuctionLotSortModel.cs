using FamilyNet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.ViewModels.AuctionLot
{
    public class AuctionLotSortModel
    {
        public SortState NameSort { get; private set; } // значение для сортировки по имени
        public SortState AgeSort { get; private set; }    // значение для сортировки по возрасту
        public SortState CompanySort { get; private set; }   // значение для сортировки по компании
        public SortState Current { get; private set; }     // текущее значение сортировки

        public AuctionLotSortModel(SortState sortOrder)
        {
            NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            AgeSort = sortOrder == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;
            Current = sortOrder;
        }
    }
}
