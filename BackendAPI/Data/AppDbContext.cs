using Microsoft.EntityFrameworkCore;
using BackendAPI.Models;

namespace BackendAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        // Master Data
        public DbSet<Product> Products { get; set; }
        public DbSet<RawMaterial> RawMaterials { get; set; } 
        public DbSet<Bom> BOMs { get; set; }
        // Inventories
        public DbSet<RawMaterialInventory> RawMaterialInventories { get; set; } 
        public DbSet<FinishedGoodsInventory> FinishedGoodsInventories { get; set; }

        // Orders
        public DbSet<SalesOrder> SalesOrders { get; set; }
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
            // --- Roles & Users ---
            modelBuilder.Entity<Role>()
            .HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId);
            // CONFIGURING AUDITABLE ENTITIES (UpdatedBy Relationships)
            // We use HasOne<User>() so it works even if you didn't add 
            // the Navigation Property "UpdatedByUser" in every class.

            ConfigureAudit<Product>(modelBuilder);
            ConfigureAudit<Bom>(modelBuilder);
            ConfigureAudit<RawMaterial>(modelBuilder);
            ConfigureAudit<SalesOrder>(modelBuilder);
            ConfigureAudit<ProductionOrder>(modelBuilder);
            ConfigureAudit<RawMaterialInventory>(modelBuilder);
            ConfigureAudit<FinishedGoodsInventory>(modelBuilder);

          
            // SPECIFIC RELATIONSHIPS

            // ProductionOrder 
            modelBuilder.Entity<ProductionOrder>()
                .HasOne(p => p.SalesOrder)
                .WithMany()
                .HasForeignKey(p => p.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<ProductionOrder>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<ProductionOrder>()
                .HasOne(p => p.CreatedByUser)
                .WithMany(u => u.ProductionOrders)
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // BOM (Soft Delete hone only active pr unique contraint lagao)
            modelBuilder.Entity<Bom>()
                .HasIndex(b => new { b.ProductId, b.RawMaterialId }) 
                .IsUnique()
                .HasFilter("[IsActive] = 1"); 

            modelBuilder.Entity<Bom>()
                .HasOne(b => b.Product)
                .WithMany(p => p.Boms)
                .HasForeignKey(b => b.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bom>()
                .HasOne(b => b.RawMaterial)
                .WithMany()
                .HasForeignKey(b => b.RawMaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bom>()
                .HasOne(b => b.CreatedByUser)
                .WithMany() // Assuming BOM creation history isn't needed in User object
                .HasForeignKey(b => b.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // SalesOrder
            modelBuilder.Entity<SalesOrder>()
                .HasOne(s => s.CreatedByUser)
                .WithMany()
                .HasForeignKey(s => s.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // FinishedGoodsInventory 
            modelBuilder.Entity<FinishedGoodsInventory>()
                .HasOne(f => f.Product)      
                .WithMany()                  
                .HasForeignKey(f => f.ProductId) 
                .OnDelete(DeleteBehavior.Restrict); 

            // RawMaterialInventory 
            modelBuilder.Entity<RawMaterial>()
                .HasOne(r => r.Inventory)          
                .WithOne(i => i.RawMaterial)       
                .HasForeignKey<RawMaterialInventory>(i => i.RawMaterialId);

            // Precision Rules 
            modelBuilder.Entity<Bom>().Property(b => b.QuantityRequired).HasPrecision(18, 2);
            modelBuilder.Entity<RawMaterialInventory>().Property(b => b.AvailableQuantity).HasPrecision(18, 2);
            modelBuilder.Entity<FinishedGoodsInventory>().Property(b => b.AvailableQuantity).HasPrecision(18, 2);
            modelBuilder.Entity<ProductionOrder>().Property(b => b.PlannedQuantity).HasPrecision(18, 2);
            modelBuilder.Entity<ProductionOrder>().Property(b => b.ProducedQuantity).HasPrecision(18, 2);
            modelBuilder.Entity<ProductionOrder>().Property(b => b.ScrapQuantity).HasPrecision(18, 2);
        }

        //  Helper to avoid writing the same code multiple times
        private void ConfigureAudit<T>(ModelBuilder modelBuilder) where T : AuditableEntity
        {
            // CreatedBy Relationship
            modelBuilder.Entity<T>()
                .HasOne(e => e.CreatedByUser) 
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict); 

            // UpdatedBy Relationship
            modelBuilder.Entity<T>()
                .HasOne(e => e.UpdatedByUser) 
                .WithMany()
                .HasForeignKey(e => e.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}