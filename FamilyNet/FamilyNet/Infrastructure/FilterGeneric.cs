using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FamilyNet.Infrastructure
{
    public class FilterGeneric<T> //TODO: Implement this 
    {
        /// <summary>
        /// The FilteredData() method is responsible for handling multiple filters
        /// in the complex type, because the above code can handle multiple filters
        /// at the same time. Since I have used generics here, I need to use 
        /// the reflection to get the property name of any complex type.
        /// </summary>
        /// <param name="filterParams"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetFilteredData(IEnumerable<FilterParams> filterParams,
            IEnumerable<T> data)
        {
            IEnumerable<string> distinctColumns = filterParams
                .Where(x => !String.IsNullOrEmpty(x.ColumnName))
                .Select(x => x.ColumnName).Distinct();

            foreach (string colName in distinctColumns)
            {
                var filterColumn = typeof(T).GetProperty(colName,
                    BindingFlags.IgnoreCase
                        | BindingFlags.Instance
                        | BindingFlags.Public);

                if (filterColumn != null)
                {
                    IEnumerable<FilterParams> filterValues = filterParams
                        .Where(x => x.ColumnName.Equals(colName)).Distinct();

                    if (filterValues.Count() > 1)
                    {
                        IEnumerable<T> sameColData = Enumerable.Empty<T>();

                        foreach (var val in filterValues)
                            sameColData = sameColData
                                .Concat(GetFilterData(val.FilterOptions, data,
                                filterColumn, val.FilterValue));

                        data = data.Intersect(sameColData);
                    }
                    else
                        data = GetFilterData(filterValues.FirstOrDefault().FilterOptions,
                            data, filterColumn, filterValues.FirstOrDefault().FilterValue);
                }
            }
            return data;
        }

        /// <summary>
        /// FilterData() method will actually filter the data based on FilterOptions 
        /// using reflection. Some filter options are applicable to specific data types.
        /// Like for string data type we use starts with, ends with, contains etc.
        /// </summary>
        /// <param name="filterOption"></param>
        /// <param name="data"></param>
        /// <param name="filterColumn"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        private static IEnumerable<T> GetFilterData(FilterOptions filterOption,
            IEnumerable<T> data, PropertyInfo filterColumn, string filterValue)
        {
            int outValue;
            DateTime dateValue;
            switch (filterOption)
            {
                #region [StringDataType]  

                case FilterOptions.StartsWith:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null
                        && filterColumn.GetValue(x, null).ToString().ToLower()
                        .StartsWith(filterValue.ToString().ToLower())).ToList();
                    break;
                case FilterOptions.EndsWith:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null
                        && filterColumn.GetValue(x, null).ToString().ToLower()
                        .EndsWith(filterValue.ToString().ToLower())).ToList();
                    break;
                case FilterOptions.Contains:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null
                        && filterColumn.GetValue(x, null).ToString().ToLower()
                        .Contains(filterValue.ToString().ToLower())).ToList();
                    break;
                case FilterOptions.DoesNotContain:
                    data = data.Where(x => filterColumn.GetValue(x, null) == null
                        || (filterColumn.GetValue(x, null) != null && !filterColumn.GetValue(x, null)
                        .ToString().ToLower().Contains(filterValue.ToString().ToLower()))).ToList();
                    break;
                case FilterOptions.IsEmpty:
                    data = data.Where(x => filterColumn.GetValue(x, null) == null
                        || (filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null)
                        .ToString() == string.Empty)).ToList();
                    break;
                case FilterOptions.IsNotEmpty:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null
                        && filterColumn.GetValue(x, null).ToString() != string.Empty).ToList();
                    break;
                #endregion

                #region [Custom]  

                case FilterOptions.IsGreaterThan:
                    if ((filterColumn.PropertyType == typeof(Int32)
                        || filterColumn.PropertyType == typeof(Nullable<Int32>))
                        && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn
                            .GetValue(x, null)) > outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>))
                            && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn
                            .GetValue(x, null)) > dateValue).ToList();
                    }
                    break;

                case FilterOptions.IsGreaterThanOrEqualTo:
                    if ((filterColumn.PropertyType == typeof(Int32) 
                            || filterColumn.PropertyType == typeof(Nullable<Int32>)) 
                            && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => 
                        Convert.ToInt32(filterColumn.GetValue(x, null)) >= outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) 
                            && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => 
                        Convert.ToDateTime(filterColumn.GetValue(x, null)) >= dateValue).ToList();
                        break;
                    }
                    break;

                case FilterOptions.IsLessThan:
                    if ((filterColumn.PropertyType == typeof(Int32) 
                            || filterColumn.PropertyType == typeof(Nullable<Int32>)) 
                            && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => 
                        Convert.ToInt32(filterColumn.GetValue(x, null)) < outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) 
                        && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => 
                        Convert.ToDateTime(filterColumn.GetValue(x, null)) < dateValue).ToList();
                        break;
                    }
                    break;

                case FilterOptions.IsLessThanOrEqualTo:
                    if ((filterColumn.PropertyType == typeof(Int32) 
                            || filterColumn.PropertyType == typeof(Nullable<Int32>)) 
                            && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => 
                        Convert.ToInt32(filterColumn.GetValue(x, null)) <= outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) 
                            && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => 
                        Convert.ToDateTime(filterColumn.GetValue(x, null)) <= dateValue).ToList();
                        break;
                    }
                    break;

                case FilterOptions.IsEqualTo:
                    if (filterValue == string.Empty)
                    {
                        data = data.Where(x => filterColumn.GetValue(x, null) == null
                                || (filterColumn.GetValue(x, null) != null 
                                        && filterColumn.GetValue(x, null).ToString()
                                        .ToLower() == string.Empty)).ToList();
                    }
                    else
                    {
                        if ((filterColumn.PropertyType == typeof(Int32) 
                                || filterColumn.PropertyType == typeof(Nullable<Int32>)) 
                                && Int32.TryParse(filterValue, out outValue))
                        {
                            data = data.Where(x => 
                            Convert.ToInt32(filterColumn.GetValue(x, null)) == outValue).ToList();
                        }
                        else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) 
                                && DateTime.TryParse(filterValue, out dateValue))
                        {
                            data = data.Where(x => 
                            Convert.ToDateTime(filterColumn.GetValue(x, null)) == dateValue).ToList();
                            break;
                        }
                        else
                        {
                            data = data.Where(x => filterColumn.GetValue(x, null) != null 
                                && filterColumn.GetValue(x, null).ToString()
                                .ToLower() == filterValue.ToLower()).ToList();
                        }
                    }
                    break;

                case FilterOptions.IsNotEqualTo:
                    if ((filterColumn.PropertyType == typeof(Int32) 
                            || filterColumn.PropertyType == typeof(Nullable<Int32>)) 
                            && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => 
                        Convert.ToInt32(filterColumn.GetValue(x, null)) != outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) 
                            && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => 
                        Convert.ToDateTime(filterColumn.GetValue(x, null)) != dateValue).ToList();
                        break;
                    }
                    else
                    {
                        data = data.Where(x => filterColumn.GetValue(x, null) == null 
                                || (filterColumn.GetValue(x, null) != null 
                                    && filterColumn.GetValue(x, null)
                                    .ToString().ToLower() != filterValue.ToLower())).ToList();
                    }
                    break;
                    #endregion
            }
            return data;
        }
    }
}
