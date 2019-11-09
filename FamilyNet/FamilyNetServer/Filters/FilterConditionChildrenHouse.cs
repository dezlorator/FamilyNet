using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.Models;

namespace FamilyNetServer.Filters
{
    public class FilterConditionChildrenHouse : IFilterConditionsChildrenHouse
    {
        public IQueryable<Orphanage> GetFilteredChildrenHouses(IQueryable<Orphanage> childrenHouses, string name, float rating, string address)
        {
            if (!String.IsNullOrEmpty(name))
            {
                childrenHouses = childrenHouses.Where(c => c.Name.ToUpper().Contains(name.ToUpper()) 
                || Contains(c.Adress, address));
            }

            if (rating > 0.0)
            {
                childrenHouses = childrenHouses.Where(c => c.Rating > rating);
            }

            if (!String.IsNullOrEmpty(address))
            {
                childrenHouses = childrenHouses.Where(c => Contains(c.Adress, address) 
                || c.Name.ToUpper().Contains(name.ToUpper()));
            }

            return childrenHouses;
        }

        public static bool Contains(Address addr, string searchName)
        {
            searchName = searchName.ToUpper();

            return (addr.Street.ToUpper().Contains(searchName)
                        || addr.City.ToUpper().Contains(searchName)
                        || addr.Region.ToUpper().Contains(searchName)
                        || addr.Country.ToUpper().Contains(searchName));
        }

        public IQueryable<Orphanage> GetSortedChildrenHouses(IQueryable<Orphanage> childrenHouses, string sort)
        {
            if (!String.IsNullOrEmpty(sort))
            {
                Enum.TryParse(typeof(SortState), sort, out var sortSate);
                switch (sortSate)
                {
                    case SortState.NameDesc:
                        childrenHouses = childrenHouses.OrderByDescending(s => s.Name);
                        break;
                    case SortState.AddressAsc:
                        childrenHouses = childrenHouses
                            .OrderBy(s => s.Adress.Country)
                            .ThenBy(s => s.Adress.Region)
                            .ThenBy(s => s.Adress.City)
                            .ThenBy(s => s.Adress.Street);
                        break;
                    case SortState.AddressDesc:
                        childrenHouses = childrenHouses
                            .OrderByDescending(s => s.Adress.Country)
                            .ThenByDescending(s => s.Adress.Region)
                            .ThenByDescending(s => s.Adress.City)
                            .ThenByDescending(s => s.Adress.Street);
                        break;
                    case SortState.RatingAsc:
                        childrenHouses = childrenHouses.OrderBy(s => s.Rating);
                        break;
                    case SortState.RatingDesc:
                        childrenHouses = childrenHouses.OrderByDescending(s => s.Rating);
                        break;
                    default:
                        childrenHouses = childrenHouses.OrderBy(s => s.Name);
                        break;
                }
            }

            return childrenHouses;
        }
    }
}
