using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FamilyNetServer.Models;

namespace FamilyNetServer.Filters
{ 
    public interface ICharityMakersSelection
    {
        IQueryable<CharityMaker> GetFiltered(IQueryable<CharityMaker> charityMakersCollection, 
            string fullName, float rating);
    }
}
