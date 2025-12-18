using BackendAPI.Models;

namespace BackendAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            Console.WriteLine("🚀 Seeding Process Started...");

            // 1. DB Check
            context.Database.EnsureCreated();

            // 2. Data Check
            if (context.Users.Any() && context.SalesOrders.Any())
            {
                Console.WriteLine("✅ Database already has data. Seeding Skipped.");
                return;
            }

            Console.WriteLine("⚡ No complete data found. Seeding now...");

            // ==========================================
            // 1. ROLES
            // ==========================================
            if (!context.Roles.Any())
            {
                var adminRole = new Role { RoleName = "Admin" };
                var salesRole = new Role { RoleName = "Sales" };
                var prodRole = new Role { RoleName = "Production" };

                context.Roles.AddRange(adminRole, salesRole, prodRole);
                context.SaveChanges();
                Console.WriteLine("   > Roles Created");
            }

            // ==========================================
            // 2. USERS
            // ==========================================
            if (!context.Users.Any())
            {
                var adminRole = context.Roles.First(r => r.RoleName == "Admin");
                var salesRole = context.Roles.First(r => r.RoleName == "Sales");
                var prodRole = context.Roles.First(r => r.RoleName == "Production");

                var adminUser = new User 
                { 
                    Username = "SystemAdmin", 
                    Email = "admin@erp.com", 
                    PasswordHash = "admin123", 
                    RoleId = adminRole.RoleId,
                    CreatedAt = DateTime.Now // User Auditable nahi hai, manually date daal rahe hain
                };

                var salesUser = new User 
                { 
                    Username = "Rahul_Sales", 
                    Email = "rahul@erp.com", 
                    PasswordHash = "sales123", 
                    RoleId = salesRole.RoleId,
                    CreatedAt = DateTime.Now
                };

                var prodUser = new User 
                { 
                    Username = "Vikram_Factory", 
                    Email = "vikram@erp.com", 
                    PasswordHash = "prod123", 
                    RoleId = prodRole.RoleId,
                    CreatedAt = DateTime.Now
                };

                context.Users.AddRange(adminUser, salesUser, prodUser);
                context.SaveChanges(); 
                Console.WriteLine("   > Users Created");
            }

            // --- GET USER IDs ---
            var sysAdminId = context.Users.First(u => u.Username == "SystemAdmin").UserId;
            var salesUserId = context.Users.First(u => u.Username == "Rahul_Sales").UserId;
            var prodUserId = context.Users.First(u => u.Username == "Vikram_Factory").UserId;


            // ==========================================
            // 3. RAW MATERIALS & INVENTORY
            // ==========================================
            if (!context.RawMaterials.Any())
            {
                // FIX: Added CreatedByUserId
                var wood = new RawMaterial 
                { 
                    Name = "Teak Wood", 
                    SKU = "RM-WOOD-01", 
                    CreatedByUserId = sysAdminId // <--- ADDED THIS (Required)
                };
                
                var glue = new RawMaterial 
                { 
                    Name = "Super Glue", 
                    SKU = "RM-GLUE-01", 
                    CreatedByUserId = sysAdminId // <--- ADDED THIS (Required)
                };

                context.RawMaterials.AddRange(wood, glue);
                context.SaveChanges(); 

                // Inventory creation
                context.RawMaterialInventories.AddRange(
                    new RawMaterialInventory 
                    { 
                        RawMaterialId = wood.RawMaterialId, 
                        AvailableQuantity = 100,
                        CreatedByUserId = prodUserId, // <--- ADDED THIS (Required)
                        UpdatedByUserId = prodUserId 
                    },
                    new RawMaterialInventory 
                    { 
                        RawMaterialId = glue.RawMaterialId, 
                        AvailableQuantity = 50,
                        CreatedByUserId = prodUserId, // <--- ADDED THIS (Required)
                        UpdatedByUserId = prodUserId 
                    }
                );
                Console.WriteLine("   > Raw Materials & Inventory Created");
            }

            // ==========================================
            // 4. PRODUCT & FG INVENTORY
            // ==========================================
            if (!context.Products.Any())
            {
                var chair = new Product 
                { 
                    Name = "FG-CHAIR-001",
                    CreatedByUserId = sysAdminId 
                };
                
                context.Products.Add(chair);
                context.SaveChanges(); 

                // FG Inventory
                context.FinishedGoodsInventories.Add(
                    new FinishedGoodsInventory 
                    { 
                        ProductId = chair.ProductId, 
                        AvailableQuantity = 0,
                        CreatedByUserId = prodUserId, // <--- ADDED THIS (Required)
                        UpdatedByUserId = prodUserId 
                    }
                );
                
                // BOM Creation
                var woodId = context.RawMaterials.First(r => r.SKU == "RM-WOOD-01").RawMaterialId;
                var glueId = context.RawMaterials.First(r => r.SKU == "RM-GLUE-01").RawMaterialId;

                context.BOMs.AddRange(
                    new Bom 
                    { 
                        ProductId = chair.ProductId, 
                        RawMaterialId = woodId, 
                        QuantityRequired = 2, 
                        CreatedByUserId = sysAdminId 
                    },
                    new Bom 
                    { 
                        ProductId = chair.ProductId, 
                        RawMaterialId = glueId, 
                        QuantityRequired = 1, 
                        CreatedByUserId = sysAdminId 
                    }
                );
                context.SaveChanges();
                Console.WriteLine("   > Products, FG Inventory & BOM Created");
            }

            // ==========================================
            // 5. SALES ORDER
            // ==========================================
            if (!context.SalesOrders.Any())
            {
                var chairId = context.Products.First(p => p.Name == "FG-CHAIR-001").ProductId;

                var order1 = new SalesOrder
                {
                    CustomerName = "TechCorp Solutions",
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    ProductId = chairId,
                    Quantity = 10,
                    CreatedByUserId = salesUserId 
                };

                context.SalesOrders.Add(order1);
                context.SaveChanges();
                Console.WriteLine("   > Sales Order Created");
            }
            
            Console.WriteLine("✅ Seeding Completed Successfully!");
        }
    }
}