using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies();
            //.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=FamilyNetEFCore;Trusted_Connection=True;");
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //}



    }

    public class ApplicationDbContextFactory
            : IDesignTimeDbContextFactory<ApplicationDbContext>
    {

        public ApplicationDbContext CreateDbContext(string[] args) =>
            Program.BuildWebHost(args).Services
                .GetRequiredService<ApplicationDbContext>();
    }
}
