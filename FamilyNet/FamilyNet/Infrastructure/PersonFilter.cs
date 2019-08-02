using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNet.Models;
using FamilyNet.Models.ViewModels;

namespace FamilyNet.Infrastructure
{
    public static class PersonFilter
    {
        private static PersonSearchModel _searchModel;

        public static IQueryable<Person> GetFiltered(this IQueryable<Person> persons, PersonSearchModel searchModel)
        {
            if (searchModel != null)
            {
                _searchModel = searchModel;

                if (!string.IsNullOrEmpty(searchModel.FullNameString))
                    persons = persons.Where(x => IsContain(x.FullName));

                if (searchModel.RatingNumber > 0)
                    persons = persons.Where(x => x.Rating == searchModel.RatingNumber);
            }
            //TODO: REturn rersons.Where().Where;
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
