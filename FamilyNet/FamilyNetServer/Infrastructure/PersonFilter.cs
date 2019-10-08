using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.Models;
using FamilyNetServer.Models.Interfaces;
using FamilyNetServer.Models.ViewModels;

namespace FamilyNetServer.Infrastructure
{
    public static class PersonFilter
    {
        private static PersonSearchModel _searchModel;

        //TODO: AlPa -> GENERIC Sorting+filtered in asp.net mvc
        public static IQueryable<Person> GetFiltered(this IQueryable<Person> persons, PersonSearchModel searchModel)
        {
            if (searchModel != null)
            {
                _searchModel = searchModel;

                if (!string.IsNullOrEmpty(searchModel.FullNameString))
                    persons = persons.Where(x => IsContain(x.FullName));

                //TODO: Check type of RNumber for testing change to string
                if (searchModel.RatingNumber > 0)
                    persons = persons.Where(x => x.Rating >= searchModel.RatingNumber);
            }
            //TODO: AlPa -> REturn rersons.Where().Where;
            return persons;
        }

        private static bool IsContain(FullName fullname)
        {
            foreach (var word in _searchModel.FullNameString.Split())
            {
                string wordUpper = word.ToUpper();

                if (fullname.Name.ToUpper().Contains(wordUpper)
                        || fullname.Surname.ToUpper().Contains(wordUpper)
                        || fullname.Patronymic.ToUpper().Contains(wordUpper))
                    return true;
            }

            return false;
        }
    }
}
