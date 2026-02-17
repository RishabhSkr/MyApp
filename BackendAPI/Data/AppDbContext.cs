using Microsoft.EntityFrameworkCore;
using BackendAPI.Models;

namespace BackendAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Master Data
        public DbSet<Bom> BOMs { get; set; }
        // Orders
        public DbSet<ProductionOrder> ProductionOrders { get; set; }

     
        // Save Date & User automatically
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<AuditableEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.Now;
                    entry.Entity.IsActive = true;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.Now;
                    
                    // Protect Creation Data
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    entry.Property(x => x.CreatedByUserId).IsModified = false;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

      
        // CONFIGURATION & RELATIONSHIPS: fluentApi Rules and Valiations

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // SPECIFIC RELATIONSHIPS
            // ProductionOrder 
            modelBuilder.Entity<ProductionOrder>(e => {
                e.HasKey(p => p.ProductionOrderId);
                e.Property(p => p.PlannedQuantity).HasPrecision(18, 2);
                e.Property(p => p.ProducedQuantity).HasPrecision(18, 2);
                e.Property(p => p.ScrapQuantity).HasPrecision(18, 2);
            });

            // BOM (Soft Delete hone only active pr unique contraint lagao)
            modelBuilder.Entity<Bom>(e => {
                e.HasKey(b => b.Id);
                e.Property(b => b.QuantityRequired).HasPrecision(18, 2);
                e.HasIndex(b => new { b.ProductId, b.RawMaterialId })
                    .IsUnique()
                    .HasFilter("[IsActive] = 1");
            });                               
        }
    }
}