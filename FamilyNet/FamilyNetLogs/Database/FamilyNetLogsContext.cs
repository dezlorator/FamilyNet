using FamilyNetLogs.Models;
using Microsoft.EntityFrameworkCore;

namespace FamilyNetLogs.Database
{
    public partial class FamilyNetLogsContext : DbContext
    {
        public virtual DbSet<Log> Log { get; set; }

        public FamilyNetLogsContext(DbContextOptions<FamilyNetLogsContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.CallSite).HasMaxLength(256);
                entity.Property(e => e.UserId).HasMaxLength(50);
                entity.Property(e => e.Exception).HasColumnType("text");
                entity.Property(e => e.Exception).HasColumnType("text");
                entity.Property(e => e.JSON).HasColumnType("text");
                entity.Property(e => e.Token).HasColumnType("text");
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.Level).HasMaxLength(256);
                entity.Property(e => e.Logger).HasMaxLength(256);
                entity.Property(e => e.Info).HasMaxLength(256);

                entity.Property(e => e.Logged)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
        }
    }
}
