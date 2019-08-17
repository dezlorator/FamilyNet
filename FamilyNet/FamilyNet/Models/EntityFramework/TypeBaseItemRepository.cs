using FamilyNet.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNet.Models.EntityFramework
{
    public class TypeBaseItemRepository // TODO : Rewrite this class
    {
        protected DbSet<TypeBaseItem> dbSet;

        public TypeBaseItemRepository(ApplicationDbContext dbContext)
        {
            dbSet = dbContext.TypeBaseItems;
        }

        public IQueryable GetAll()
        {
            return dbSet;

        }

        public async Task Create(TypeBaseItem entity)
        {
            await dbSet.AddAsync(entity);

        }

        public void Update(TypeBaseItem entity)
        {
            dbSet.Update(entity);
        }

        public async Task<TypeBaseItem> GetById(int itemID, int typeId)
        {
            return await dbSet.FirstOrDefaultAsync(tbt => tbt.ItemID == itemID && tbt.TypeID == typeId);
        }

        public IEnumerable<TypeBaseItem> Get(Func<TypeBaseItem, bool> predicate)
        {
            return dbSet.Where(predicate);
        }

        public void AddRange(IEnumerable<TypeBaseItem> entities)
        {
            dbSet.AddRange(entities);
        }

        public virtual async Task Delete(int itemID, int typeId)
        {
            var entity = GetById(itemID, typeId).Result;
            dbSet.Remove(entity);
        }

    }
}
