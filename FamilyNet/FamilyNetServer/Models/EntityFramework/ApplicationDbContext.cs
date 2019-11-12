using FamilyNetServer.Models.EntityFramework.Fluent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyNetServer.Models.EntityFramework
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Address> Address { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<Orphan> Orphans { get; set; }
        public DbSet<CharityMaker> CharityMakers { get; set; }
        public DbSet<Representative> Representatives { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Orphanage> Orphanages { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<TypeBaseItem> TypeBaseItems { get; set; }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<AuctionLot> AuctionLot { get; set; }
        public DbSet<ChildActivity> Activities { get; set; }
        public DbSet<Award> Awards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                .OnDelete(DeleteBehavior.ClientSetNull);

            #region Lots and Donation

            //one to many
            modelBuilder.Entity<AuctionLotItem>()
                .HasMany<AuctionLotItemType>(a => a.AuctionLotItemTypes)
                .WithOne(b => b.Item)
                .HasForeignKey(k => k.ItemID)
                .OnDelete(DeleteBehavior.ClientSetNull);



            modelBuilder.Entity<DonationItem>()
                .HasOne<Donation>()
                .WithOne(d => d.DonationItem)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Donation>()
                .HasIndex(i => i.CharityMakerID);

            modelBuilder.Entity<Donation>()
                .HasIndex(i => i.OrphanageID);

            #endregion

            modelBuilder.Entity<Orphan>()
                .HasMany<AuctionLot>(r => r.AuctionLots)
                .WithOne(b => b.Orphan)
                .HasForeignKey(fk => fk.OrphanID)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<TypeBaseItem>()
          .HasKey(t => new { t.ItemID, t.TypeID});;

            modelBuilder.Entity<TypeBaseItem>()
                .HasOne(tbi => tbi.Item )
                .WithMany(i => i.TypeBaseItem)
                .HasForeignKey(tbi => tbi.ItemID);

            modelBuilder.Entity<TypeBaseItem>()
                .HasOne(tbi => tbi.Type)
                .WithMany(t => t.TypeBaseItem)
                .HasForeignKey(tbi => tbi.TypeID);

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

            #region SoftDeleteSetUp

            modelBuilder.Entity<Address>().HasQueryFilter(entity => !entity.IsDeleted);
            modelBuilder.Entity<CharityMaker>().HasQueryFilter(entity => !entity.IsDeleted);
            modelBuilder.Entity<Orphan>().HasQueryFilter(entity => !entity.IsDeleted);
            modelBuilder.Entity<Volunteer>().HasQueryFilter(entity => !entity.IsDeleted);
            modelBuilder.Entity<Representative>().HasQueryFilter(entity => !entity.IsDeleted);
            modelBuilder.Entity<Orphanage>().HasQueryFilter(entity => !entity.IsDeleted);
            modelBuilder.Entity<AuctionLot>().HasQueryFilter(entity => !entity.IsDeleted);
            modelBuilder.Entity<BaseItem>().HasQueryFilter(entity => !entity.IsDeleted);
            modelBuilder.Entity<BaseItemType>().HasQueryFilter(entity => !entity.IsDeleted);
            modelBuilder.Entity<Donation>().HasQueryFilter(entity => !entity.IsDeleted);
            
            modelBuilder.Entity<Location>().HasQueryFilter(entity => !entity.IsDeleted);

            #endregion
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
