using FamilyNet.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.ViewModels
{
    public class RepresentativeViewModel
    {
        public IEnumerable<Representative> Representatives { get; set; }
        public FilterModel FilterModel { get; set; }

        public RepresentativeViewModel()
        {
            Representatives = new List<Representative>();
            FilterModel = new FilterModel();
            AddParams();
        }

        public void AddParams()
        {
            FilterModel.FilterParam = new List<FilterParams>()
                {
                        new FilterParams() { ColumnName = "Name", FilterOptions = FilterOptions.Contains },
                        new FilterParams() { ColumnName = "Surname", FilterOptions = FilterOptions.Contains },
                        new FilterParams() { ColumnName = "Patronymic", FilterOptions = FilterOptions.Contains },
                        new FilterParams() { ColumnName = "Rating", FilterOptions = FilterOptions.IsGreaterThanOrEqualTo }
                };
        }
    }
}
