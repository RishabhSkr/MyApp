// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using BackendAPI.Models;

namespace BackendAPI.Data
{
    // This class inherits from DbContext (built-in .NET tool)
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        // Master Data for ProdcutionOrders
        // This tells .NET: "I want a table called 'Products(Collection)' based on the 'Product' class"
        public DbSet<Product> Products { get; set; }
        public DbSet<RawMaterial> RawMaterials { get; set; } 
        public DbSet<Bom> BOM { get; set; }

        // Inventories
        public DbSet<RawMaterialInventory> RawMaterialInventories { get; set; } 
        public DbSet<FinishedGoodsInventory> FinishedGoodsInventories { get; set; }

        //  Orders
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<ProductionOrder> ProductionOrders { get; set; }

        // FluentAPI Validation Rules
       protected override void OnModelCreating(ModelBuilder modelBuilder)
{
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId);
                
            // PRODUCTION ORDER RELATIONSHIPS
        
            modelBuilder.Entity<ProductionOrder>()
                .HasOne(p => p.SalesOrder)
                .WithMany()
                .HasForeignKey(p => p.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<ProductionOrder>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId) // Ensure small 'd'
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<ProductionOrder>()
                .HasOne(p => p.CreatedByUser)
                .WithMany(u => u.ProductionOrders)
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict); 

        
            // 2. BOM RELATIONSHIPS (THE FIX)
            // Composite Key: A Product cannot have duplicate RawMaterials
            modelBuilder.Entity<Bom>()
                .HasIndex(b => new { b.ProductId, b.RawMaterialId }) 
                .IsUnique();

            // Link to Product
            modelBuilder.Entity<Bom>()
                .HasOne(b => b.Product)
                .WithMany(p => p.Boms) // Product has a list of Boms
                .HasForeignKey(b => b.ProductId) // <--- CRITICAL: Must match Model (ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Changed from NoAction to Restrict for safety

            // Link to RawMaterial
            modelBuilder.Entity<Bom>()
                .HasOne(b => b.RawMaterial)
                .WithMany()
                .HasForeignKey(b => b.RawMaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            // Link to User
            modelBuilder.Entity<Bom>()
                .HasOne(b => b.CreatedByUser)
                .WithMany()
                .HasForeignKey(b => b.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict); 


            // SALES ORDER & INVENTORY
            modelBuilder.Entity<SalesOrder>()
                .HasOne(s => s.CreatedByUser)
                .WithMany() // Assuming User doesn't need a list of SalesOrders, or update User model
                .HasForeignKey(s => s.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FinishedGoodsInventory>()
                .HasOne(f => f.Product)      
                .WithMany()                  
                .HasForeignKey(f => f.ProductId) 
                .OnDelete(DeleteBehavior.Restrict); 

            // RM-Inventory (One to One Mapping)
            modelBuilder.Entity<RawMaterial>()
                .HasOne(r => r.Inventory)          
                .WithOne(i => i.RawMaterial)       
                .HasForeignKey<RawMaterialInventory>(i => i.RawMaterialId);

            // PRECISION Rules
            modelBuilder.Entity<Bom>().Property(b => b.QuantityRequired).HasPrecision(18, 2);
            modelBuilder.Entity<RawMaterialInventory>().Property(b => b.AvailableQuantity).HasPrecision(18, 2);
            modelBuilder.Entity<FinishedGoodsInventory>().Property(b => b.AvailableQuantity).HasPrecision(18, 2);
            modelBuilder.Entity<ProductionOrder>().Property(b => b.PlannedQuantity).HasPrecision(18, 2);
            modelBuilder.Entity<ProductionOrder>().Property(b => b.ProducedQuantity).HasPrecision(18, 2);
        }
    }
}