using FamilyNet.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Infrastructure
{
    public class PagingUtility<T>  //TODO: Implement this 
    {
        public async Task<PaginatedList<T>> GetPagingList(List<T> persons, FilterModel pagingParams)
        {
            List<T> sampleList = persons;

            #region [Filter]  
            if (pagingParams != null && pagingParams.FilterParam.Any())
            {
                sampleList = FilterGeneric<T>
                    .GetFilteredData(pagingParams.FilterParam, sampleList).ToList() ?? sampleList;
            }
            #endregion

            #region [Sorting]  
            if (pagingParams != null && pagingParams.SortingParams.Count() > 0
                    && Enum.IsDefined(typeof(SortingUtility.SortOrders),
                    pagingParams.SortingParams.Select(x => x.SortOrder)))
            {
                sampleList = SortingUtility.Sorting<T>
                    .SortData(sampleList, pagingParams.SortingParams).ToList();
            }
            #endregion

            #region [Grouping]  
            if (pagingParams != null && pagingParams.GroupingColumns != null
                    && pagingParams.GroupingColumns.Count() > 0)
            {
                sampleList = SortingUtility.Sorting<T>
                    .GroupingData(sampleList, pagingParams.GroupingColumns).ToList() ?? sampleList;
            }
            #endregion

            #region [Paging]  
            return await PaginatedList<T>
                .CreateAsync(sampleList, pagingParams.PageNumber, pagingParams.PageSize);
            #endregion
        }
    }
}
