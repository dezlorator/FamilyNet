using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models.EntityFramework
{
    public class PersonRepositoryAsync<TEntity> : EFRepositoryAsync<TEntity> where TEntity : Person
    {
        public PersonRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
            

    }
}
