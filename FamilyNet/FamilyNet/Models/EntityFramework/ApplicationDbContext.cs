using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyNet.Models.EntityFramework
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Orphan> Orphans { get; set; }
        public DbSet<CharityMaker> CharityMakers { get; set; }
        public DbSet<Representative> Representatives { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Orphanage> Orphanages { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<TypeBaseItem> TypeBaseItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies();
        }
    }

    public class ApplicationDbContextFactory
            : IDesignTimeDbContextFactory<ApplicationDbContext>
    {

        public ApplicationDbContext CreateDbContext(string[] args) =>
            Program.BuildWebHost(args).Services
                .GetRequiredService<ApplicationDbContext>();
    }
}
