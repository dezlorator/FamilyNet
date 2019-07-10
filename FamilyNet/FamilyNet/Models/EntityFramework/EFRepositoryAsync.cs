using FamilyNet.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.EntityFramework
{
    public class EFRepositoryAsync<TEntity> : IAsyncRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly ApplicationDbContext _dbContext;

        public EFRepositoryAsync(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get all instances TEntity from the database without tracking their changes.(it will work faster)
        /// </summary>
        /// <returns>IQueryable<TEntity></returns>
        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }
        public async Task Create(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            
        }

        public async Task Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);            
        }

        public async Task Delete(int id)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id);
            _dbContext.Set<TEntity>().Remove(entity);
            
        }

        public async Task<TEntity> GetById(int id)
        {
            return await _dbContext.Set<TEntity>()
                        .FirstOrDefaultAsync(e => e.ID == id);
        }

        public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate)
        {
            return  _dbContext.Set<TEntity>()
                        .Where(predicate);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
