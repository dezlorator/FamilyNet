using FamilyNetServer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models.EntityFramework
{
    public class OrphanageRepositoryAsync : EFRepositoryAsync<Orphanage>, IOrphanageAsyncRepository
    {
        public OrphanageRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public IQueryable<Orphanage> GetForSearchOrphanageOnMap()
        {
            var Orphanages = _dbContext.Orphanages.AsQueryable()
                .Where(c => c.Location.MapCoordX != null && c.Location.MapCoordY != null)
                .Select(c => new Orphanage
                {
                    Adress = c.Adress,
                    Name = c.Name,
                    Location = new Location {MapCoordX=c.Location.MapCoordX, MapCoordY=c.Location.MapCoordY },
                });

            return Orphanages;
        }
    }
}
