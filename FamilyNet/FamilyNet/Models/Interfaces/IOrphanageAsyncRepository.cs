using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.Interfaces
{
    public interface IOrphanageAsyncRepository: IAsyncRepository<Orphanage> 
    {
        IQueryable<Orphanage> GetForSearchOrphanageOnMap();
    }
}
