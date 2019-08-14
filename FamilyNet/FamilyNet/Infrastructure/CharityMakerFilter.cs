using FamilyNet.Models;
using FamilyNet.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Infrastructure
{
    public static class CharityMakerFilter
    {
        private static PersonSearchModel _searchModel;

        public static IEnumerable<CharityMaker> GetFiltered(this IEnumerable<CharityMaker> persons,
            PersonSearchModel searchModel)
        {
            if (searchModel != null)
            {
                _searchModel = searchModel;

                if (!string.IsNullOrEmpty(searchModel.FullNameString))
                    persons = persons.Where(x => IsContain(x.FullName));

                if (searchModel.RatingNumber > 0)
                    persons = persons.Where(x => x.Rating == searchModel.RatingNumber);
            }
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
