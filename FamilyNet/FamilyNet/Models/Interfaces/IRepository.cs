using System;
using System.Collections.Generic;

namespace FamilyNet.Models.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class, IEntity
    {
        void Create(TEntity entity);
        void Delete(int id);
        void Update(TEntity entity);

        //read side (could be in separate Readonly Generic Repository)
        TEntity GetById(int id);
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Get(Func<TEntity, bool> predicate);
        void SaveChanges();
    }
}