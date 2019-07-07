using FamilyNet.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.EntityFramework
{
    // TODO: Choose sync or async type Repository
    public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly ApplicationDbContext _context;

        public EFRepository(ApplicationDbContext context) => _context = context;

        public void Create(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Delete(int id)
        {
            var entityToDelete = _context.Set<TEntity>().FirstOrDefault(e => e.ID == id);
            if (entityToDelete != null)
            {
                _context.Set<TEntity>().Remove(entityToDelete);
            }
        }

        public void Update(TEntity entity)
        {
            var editedEntity = _context.Set<TEntity>().FirstOrDefault(e => e.ID == entity.ID);
            editedEntity = entity;
        }

        public TEntity GetById(int id)
        {
            return _context.Set<TEntity>().FirstOrDefault(e => e.ID == id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }

        public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate)
        {
            return _context.Set<TEntity>().Where(predicate);
        }

        public void SaveChanges() => _context.SaveChanges();
    }
}
