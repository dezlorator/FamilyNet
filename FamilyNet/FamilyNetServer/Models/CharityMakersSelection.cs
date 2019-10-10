using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.Models.Interfaces;

namespace FamilyNetServer.Models
{
    public class CharityMakersSelection : ICharityMakersSelection
    {
        public IQueryable<CharityMaker> GetFiltered(IQueryable<CharityMaker> charityMakersCollection, 
            string name, float rating)
        {
            if(!string.IsNullOrEmpty(name))
            {
                charityMakersCollection = charityMakersCollection
                    .Where((c => c.FullName.ToString().Contains(name)));
            }
            if(rating > 0)
            {
                charityMakersCollection = charityMakersCollection
                    .Where(c => c.Rating > rating);
            }

            return charityMakersCollection;
        }

    }
}
