using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        IQueryable<TEntity> GetAll();

        IEnumerable<TEntity> Get(Func<TEntity, bool> predicate);

        Task<TEntity> GetById(int id);

        Task Create(TEntity entity);

        void Update(TEntity entity);

        Task HardDelete(int id);

        Task Delete(int id);

        void AddRange(IEnumerable<TEntity> entities);

        //void AddRange(params TEntity[] entities);

        Task SaveChangesAsync();

        bool Any(int id);


    }
}