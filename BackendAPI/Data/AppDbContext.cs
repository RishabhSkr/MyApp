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
        public DbSet<Bom> BOMs { get; set; }

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
                .HasForeignKey(u => u.RoleID);
            
            // seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleID = 1, RoleName = "Admin" },
                new Role { RoleID = 2, RoleName = "Sales" },
                new Role { RoleID = 3, RoleName = "Production" }
            );

            
        // Diamond Problem: No Action on delete
            // Stop Cascade Delete for SalesOrder -> ProductionOrder
            modelBuilder.Entity<ProductionOrder>()
                .HasOne(p => p.SalesOrder)
                .WithMany()
                .HasForeignKey(p => p.SalesOrderID)
                .OnDelete(DeleteBehavior.Restrict); 

            //  Stop Cascade Delete for Product -> ProductionOrder
            modelBuilder.Entity<ProductionOrder>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductID)
                .OnDelete(DeleteBehavior.Restrict); 

                
            modelBuilder.Entity<FinishedGoodsInventory>()
                .HasOne(f => f.Product)       // Inventory link Product
                .WithMany()                   // one to one link 
                .HasForeignKey(f => f.ProductID) // Link via ProductID
                .OnDelete(DeleteBehavior.Restrict); // Product delete hone par Inventory record safe 
                
            // User Link
            modelBuilder.Entity<ProductionOrder>()
                .HasOne(p => p.CreatedByUser)
                .WithMany(u => u.ProductionOrders) // User ke pass list hai
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict); 

            // [Composite Key] 
            // Iska matlab: Ek BOM entry mein (ProductId + RawMaterialId) unique honge.
            // to stop duplicate entries
            modelBuilder.Entity<Bom>()
                .HasIndex(b => new { b.ProductID, b.RawMaterialId }).IsUnique();

            // in RM delete krne pr saare jagah jaha use hua  whan se delete na ho
            modelBuilder.Entity<Bom>()
                .HasOne(b => b.RawMaterial)
                .WithMany()
                .HasForeignKey(b => b.RawMaterialId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // RM-Inventory (One to One Mapping)
            modelBuilder.Entity<RawMaterial>()
                .HasOne(r => r.Inventory)          // RawMaterial has One Inventory
                .WithOne(i => i.RawMaterial)       // Inventory has One RawMaterial
                .HasForeignKey<RawMaterialInventory>(i => i.RawMaterialID); // fetched from RawMaterialInventory
            
            // decimal precision rules
            modelBuilder.Entity<Bom>()
                .Property(b => b.QuantityRequired)
                .HasPrecision(18, 2);

            modelBuilder.Entity<RawMaterialInventory>()
                .Property(b => b.AvailableQuantity)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<FinishedGoodsInventory>()
                .Property(b => b.AvailableQuantity)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<ProductionOrder>()
                .Property(b => b.PlannedQuantity)
                .HasPrecision(18, 2);
            
            modelBuilder.Entity<ProductionOrder>()
                .Property(b => b.ProducedQuantity)
                .HasPrecision(18, 2);
        }
    }
}