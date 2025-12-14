using BackendAPI.Models;

namespace BackendAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            Console.WriteLine("🚀 Seeding Process Started...");

            // Ensure DB is created (Safe check Migration performed)
            context.Database.EnsureCreated();

            //  CHECK: Sirf tab return karo agar Orders bhi exist karte hain.
            // Agar Users hain par Orders nahi, to hum aage badhenge.
            if (context.Users.Any() && context.SalesOrders.Any())
            {
                Console.WriteLine("✅ Database already has data. Seeding Skipped.");
                return;
            }

            Console.WriteLine("⚡ No complete data found. Seeding now...");

            // ==========================================
            // STEP 1: ROLES
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
            // STEP 2: USERS
            // ==========================================
            if (!context.Users.Any())
            {
                // Roles wapas fetch karo taaki ID sahi mile
                var adminRole = context.Roles.First(r => r.RoleName == "Admin");
                var salesRole = context.Roles.First(r => r.RoleName == "Sales");
                var prodRole = context.Roles.First(r => r.RoleName == "Production");

                var adminUser = new User { Username = "SystemAdmin", Email = "admin@erp.com", PasswordHash = "admin123", RoleID = adminRole.RoleID };
                var salesUser = new User { Username = "Rahul_Sales", Email = "rahul@erp.com", PasswordHash = "sales123", RoleID = salesRole.RoleID };
                var prodUser = new User { Username = "Vikram_Factory", Email = "vikram@erp.com", PasswordHash = "prod123", RoleID = prodRole.RoleID };

                context.Users.AddRange(adminUser, salesUser, prodUser);
                context.SaveChanges();
                Console.WriteLine("   > Users Created");
            }

            // ==========================================
            // STEP 3: RAW MATERIALS & INVENTORY
            // ==========================================
            if (!context.RawMaterials.Any())
            {
                var wood = new RawMaterial { Name = "Teak Wood", SKU = "RM-WOOD-01" };
                var glue = new RawMaterial { Name = "Super Glue", SKU = "RM-GLUE-01" };

                context.RawMaterials.AddRange(wood, glue);
                context.SaveChanges();

                context.RawMaterialInventories.AddRange(
                    new RawMaterialInventory { RawMaterialID = wood.RawMaterialID, AvailableQuantity = 100 },
                    new RawMaterialInventory { RawMaterialID = glue.RawMaterialID, AvailableQuantity = 50 }
                );
                Console.WriteLine("   > Raw Materials & Inventory Created");
            }

            // ==========================================
            // STEP 4: PRODUCT & FG INVENTORY
            // ==========================================
            if (!context.Products.Any())
            {
                var chair = new Product { Name = "FG-CHAIR-001" };
                context.Products.Add(chair);
                context.SaveChanges();

                context.FinishedGoodsInventories.Add(
                    new FinishedGoodsInventory { ProductID = chair.ProductID, AvailableQuantity = 0 }
                );
                
                // BOM (Recipe)
                // IDs fetch karne ke liye materials ko dobara query ya variable use karein
                var woodId = context.RawMaterials.First(r => r.SKU == "RM-WOOD-01").RawMaterialID;
                var glueId = context.RawMaterials.First(r => r.SKU == "RM-GLUE-01").RawMaterialID;

                context.BOMs.AddRange(
                    new Bom { ProductID = chair.ProductID, RawMaterialId = woodId, QuantityRequired = 2 },
                    new Bom { ProductID = chair.ProductID, RawMaterialId = glueId, QuantityRequired = 1 }
                );
                context.SaveChanges();
                Console.WriteLine("   > Products & BOM Created");
            }

            // ==========================================
            // STEP 5: SALES ORDER
            // ==========================================
            if (!context.SalesOrders.Any())
            {
                var chairId = context.Products.First(p => p.Name == "FG-CHAIR-001").ProductID;
                var salesUserId = context.Users.First(u => u.Username == "Rahul_Sales").UserID;

                var order1 = new SalesOrder
                {
                    CustomerName = "TechCorp Solutions",
                    OrderDate = DateTime.Now,
                    Status = "Pending Production",
                    ProductID = chairId,
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