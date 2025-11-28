using Microsoft.EntityFrameworkCore;
using StorageService.Domain.Entities;

namespace StorageService.Infrastructure.Data
{
    public class StorageDbContext : DbContext
    {
        public StorageDbContext(DbContextOptions<StorageDbContext> options) : base(options)
        {
        }

        public DbSet<StoredFile> StoredFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StoredFile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ContentType).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Container).IsRequired().HasMaxLength(100);
                entity.Property(e => e.StorageKey).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Checksum).HasMaxLength(64);
                entity.Property(e => e.OwnerId).HasMaxLength(100);
                entity.Property(e => e.TenantId).HasMaxLength(100);
                entity.Property(e => e.ExtraMetadataJson).HasColumnType("nvarchar(max)");

                entity.HasIndex(e => e.Id);
                entity.HasIndex(e => new { e.TenantId, e.DeletedAt });
                entity.HasIndex(e => new { e.OwnerId, e.DeletedAt });
            });
        }
    }
}

