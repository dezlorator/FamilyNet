using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models.Interfaces
{
    public interface ICharityMakersSelection
    {
        IQueryable<CharityMaker> GetFiltered(IQueryable<CharityMaker> charityMakersCollection, 
            string fullName, float rating);
    }
}
