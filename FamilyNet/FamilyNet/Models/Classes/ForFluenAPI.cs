using Microsoft.EntityFrameworkCore;

namespace FamilyNet.Models.Classes
{
    internal static class ForFluenAPI<T>
        where T:Person
    {
        internal static void SetFullNameRequired(ModelBuilder modelBuilder) {
            modelBuilder.Entity<T>().OwnsOne(u => u.FullName).Property(c => c.Name).IsRequired();
            modelBuilder.Entity<T>().OwnsOne(u => u.FullName).Property(c => c.Surname).IsRequired();
            modelBuilder.Entity<T>().OwnsOne(u => u.FullName).Property(c => c.Patronymic).IsRequired();
        }
    }
}
