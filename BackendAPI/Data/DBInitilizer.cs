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
                // Roles fetch
                var adminRole = context.Roles.First(r => r.RoleName == "Admin");
                var salesRole = context.Roles.First(r => r.RoleName == "Sales");
                var prodRole = context.Roles.First(r => r.RoleName == "Production");

                var adminUser = new User 
                { 
                    Username = "SystemAdmin", 
                    Email = "admin@erp.com", 
                    PasswordHash = "admin123", 
                    RoleId = adminRole.RoleId,
                };

                var salesUser = new User 
                { 
                    Username = "Rahul_Sales", 
                    Email = "rahul@erp.com", 
                    PasswordHash = "sales123", 
                    RoleId = salesRole.RoleId,
                };

                var prodUser = new User 
                { 
                    Username = "Vikram_Factory", 
                    Email = "vikram@erp.com", 
                    PasswordHash = "prod123", 
                    RoleId = prodRole.RoleId,
                };

                context.Users.AddRange(adminUser, salesUser, prodUser);
                context.SaveChanges(); // SAVE HERE TO GENERATE USER IDs
                Console.WriteLine("   > Users Created");
            }

            // --- GET USER IDs FOR AUDIT FIELDS ---
            // We need these IDs for 'CreatedByUserId' and 'UpdatedByUserId' columns
            var sysAdminId = context.Users.First(u => u.Username == "SystemAdmin").UserId;
            var salesUserId = context.Users.First(u => u.Username == "Rahul_Sales").UserId;
            var prodUserId = context.Users.First(u => u.Username == "Vikram_Factory").UserId;


            // ==========================================
            // 3. RAW MATERIALS & INVENTORY
            // ==========================================
            if (!context.RawMaterials.Any())
            {
                var wood = new RawMaterial { Name = "Teak Wood", SKU = "RM-WOOD-01" };
                var glue = new RawMaterial { Name = "Super Glue", SKU = "RM-GLUE-01" };

                context.RawMaterials.AddRange(wood, glue);
                context.SaveChanges(); // Save to get RawMaterial IDs

                // Inventory creation with UpdatedByUserId
                context.RawMaterialInventories.AddRange(
                    new RawMaterialInventory 
                    { 
                        RawMaterialId = wood.RawMaterialId, 
                        AvailableQuantity = 100,
                        UpdatedByUserId = prodUserId // Diagram field
                    },
                    new RawMaterialInventory 
                    { 
                        RawMaterialId = glue.RawMaterialId, 
                        AvailableQuantity = 50,
                        UpdatedByUserId = prodUserId // Diagram field
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
                    CreatedByUserId = sysAdminId // Diagram field
                };
                
                context.Products.Add(chair);
                context.SaveChanges(); // Save to get ProductId

                // FG Inventory with UpdatedByUserId
                context.FinishedGoodsInventories.Add(
                    new FinishedGoodsInventory 
                    { 
                        ProductId = chair.ProductId, 
                        AvailableQuantity = 0,
                        UpdatedByUserId = prodUserId // Diagram field
                    }
                );
                
                // BOM Creation
                var woodId = context.RawMaterials.First(r => r.SKU == "RM-WOOD-01").RawMaterialId;
                var glueId = context.RawMaterials.First(r => r.SKU == "RM-GLUE-01").RawMaterialId;

                // Check AppDbContext for property name (BOM or BOMs)
                context.BOM.AddRange(
                    new Bom 
                    { 
                        ProductId = chair.ProductId, 
                        RawMaterialId = woodId, 
                        QuantityRequired = 2, 
                        CreatedByUserId = sysAdminId // Diagram field
                    },
                    new Bom 
                    { 
                        ProductId = chair.ProductId, 
                        RawMaterialId = glueId, 
                        QuantityRequired = 1, 
                        CreatedByUserId = sysAdminId // Diagram field
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
                    CreatedByUserId = salesUserId // Diagram field
                };

                context.SalesOrders.Add(order1);
                context.SaveChanges();
                Console.WriteLine("   > Sales Order Created");
            }
            
            Console.WriteLine("✅ Seeding Completed Successfully!");
        }
    }
}