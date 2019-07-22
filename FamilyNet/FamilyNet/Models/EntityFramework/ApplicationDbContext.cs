using FamilyNet.Models.Classes;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder
                .UseLazyLoadingProxies();
            //.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=FamilyNetEFCore;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            //Indexes
            modelBuilder.Entity<Address>().HasIndex(o => o.City);

            //Complex types
            ForFluenAPI<CharityMaker>.SetFullNameRequired(modelBuilder);
            ForFluenAPI<Volunteer>.SetFullNameRequired(modelBuilder);
            ForFluenAPI<Representative>.SetFullNameRequired(modelBuilder);
            ForFluenAPI<Orphan>.SetFullNameRequired(modelBuilder);

            //Foreign keys
            //first part
            modelBuilder.Entity<Orphanage>()
                .HasMany<Representative>(r => r.Representatives)
                .WithOne(b => b.Orphanage)
                .HasForeignKey(fk => fk.OrphanageID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Orphanage>()
                .HasMany<Orphan>(r => r.Orphans)
                .WithOne(b => b.Orphanage)
                .HasForeignKey(fk => fk.OrphanageID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            #region Lots and Donation

            //one to many
            modelBuilder.Entity<AuctionLotItem>()
                .HasMany<AuctionLotItemType>(a => a.AuctionLotItemTypes)
                .WithOne(b => b.Item)
                .HasForeignKey(k => k.ItemID)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<DonationItem>()
               .HasMany<DonationItemType>(d => d.DonationItemTypes)
               .WithOne(b => b.Item)
               .HasForeignKey(k => k.ItemID)
               .OnDelete(DeleteBehavior.ClientSetNull);

            //one to one
            //modelBuilder.Entity<AuctionLotItem>()
            //   .HasOne<AuctionLot>()
            //   .WithOne(a => a.AuctionLotItem)
            //   .HasForeignKey<AuctionLot>(f => f.AuctionLotItemID)
            //   .OnDelete(DeleteBehavior.SetNull);

            //modelBuilder.Entity<DonationItem>()
            //   .HasOne<Donation>()
            //   .WithOne(a => a.DonationItem)
            //   .HasForeignKey<Donation>(f => f.DonationItem)
            //   .OnDelete(DeleteBehavior.SetNull);


            #endregion

            modelBuilder.Entity<Orphan>()
                .HasMany<AuctionLot>(r => r.AuctionLots)
                .WithOne(b => b.Orphan)
                .HasForeignKey(fk => fk.OrphanID)
                .OnDelete(DeleteBehavior.ClientSetNull);

            //second part
            modelBuilder.Entity<Address>()
                .HasOne<CharityMaker>()
                .WithOne(cm => cm.Address)
                .HasForeignKey<CharityMaker>(f => f.AddressID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Address>()
                .HasOne<Orphanage>()
                .WithOne(cm => cm.Adress)
                .HasForeignKey<Orphanage>(f => f.AdressID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Address>()
                .HasOne<Volunteer>()
                .WithOne(cm => cm.Address)
                .HasForeignKey<Volunteer>(f => f.AddressID)
                .OnDelete(DeleteBehavior.SetNull);
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
