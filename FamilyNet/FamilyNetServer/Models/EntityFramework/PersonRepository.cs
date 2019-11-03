namespace FamilyNetServer.Models.EntityFramework
{
    public class PersonRepository<TEntity> : EFRepository<TEntity> where TEntity : Person
    {
        public PersonRepository(ApplicationDbContext dbContext) : base(dbContext){}  
    }
}
