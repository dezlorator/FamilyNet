using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.EntityFramework
{
    public class OrphanageRepositoryAsync : EFRepositoryAsync<Orphanage>, IOrphanageAsyncRepository
    {
        public OrphanageRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            
        }
        public IQueryable<Orphanage> GetForSearchOrphanageOnMap()
        {
            var Orphanages = _dbContext.Orphanages.AsQueryable()
                .Where(c => c.MapCoordX != null && c.MapCoordY != null)
                .Select(c => new Orphanage
                {
                    Adress = c.Adress,
                    MapCoordX = c.MapCoordX,
                    MapCoordY = c.MapCoordY,
                    Name = c.Name
                });

            return Orphanages;
        }
    }
}
