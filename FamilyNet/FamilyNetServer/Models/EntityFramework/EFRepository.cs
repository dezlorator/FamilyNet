using FamilyNetServer.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models.EntityFramework {
    public class EFRepositoryAsync<TEntity> : IAsyncRepository<TEntity> where TEntity : class, IEntity {
        protected readonly ApplicationDbContext _dbContext;

        public EFRepositoryAsync(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get all instances TEntity from the database 
        /// </summary>
        /// <returns>IQueryable<TEntity></returns>
        public IQueryable<TEntity> GetAll() {
            return _dbContext.Set<TEntity>();

        }


        public async Task Create(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);

        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }

        public async Task HardDelete(int id) 
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
            return _dbContext.Set<TEntity>()
                        .Where(predicate);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
             _dbContext.Set < TEntity >().AddRange(entities);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task Delete(int id)
        {
            var entity = await GetById(id);
            entity.IsDeleted = true;
            Update(entity);
        }

        public virtual bool Any(int id)
        {
            return GetById(id) != null;
        }
    }
}
