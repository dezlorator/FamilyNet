using System.Linq;

namespace FamilyNetServer.Models.Interfaces
{
    public interface IOrphanageRepository: IRepository<Orphanage> 
    {
        IQueryable<Orphanage> GetForSearchOrphanageOnMap();
    }
}
