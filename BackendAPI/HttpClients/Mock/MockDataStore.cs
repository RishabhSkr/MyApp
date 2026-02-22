using BackendAPI.HttpClients.Dtos;

namespace BackendAPI.HttpClients.Mock;

/// <summary>
/// Shared in-memory data store for mock services
/// Controllers aur Clients dono yahi data use karenge
/// </summary>
public static class MockDataStore
{
    // ─── SALES ORDERS ───
    public static List<SalesOrderDto> SalesOrders { get; } = new()
    {
        new SalesOrderDto
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            ProductId = Guid.Parse("aaaa1111-1111-1111-1111-111111111111"),
            Quantity = 1000,
            Status = "Pending",
            OrderDate = DateTime.Now
        },
        new SalesOrderDto
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ProductId = Guid.Parse("aaaa1111-1111-1111-1111-111111111111"),
            Quantity = 1500,
            Status = "Pending",
            OrderDate = DateTime.Now
        },
        new SalesOrderDto
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            ProductId = Guid.Parse("aaaa1111-1111-1111-1111-111111111111"),
            Quantity = 1500,
            Status = "Pending",
            OrderDate = DateTime.Now
        },
        new SalesOrderDto
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            ProductId = Guid.Parse("aaaa1111-1111-1111-1111-111111111111"),
            Quantity = 500,
            Status = "Pending",
            OrderDate = DateTime.Now
        }
    };

    // ─── PRODUCTS ───
    public static List<ProductDto> Products { get; } = new()
    {
        new ProductDto
        {
            Id = Guid.Parse("aaaa1111-1111-1111-1111-111111111111"),
            Name = "Steel Widget A",
            ProductCode = "SWA",
            MaxDailyCapacity = 300
        }
    };

    // ─── RAW MATERIALS ───
    public static List<RawMaterialDto> RawMaterials { get; } = new()
    {
        new RawMaterialDto
        {
            Id = Guid.Parse("bbbb1111-1111-1111-1111-111111111111"),
            Name = "Steel Rod",
            SKU = "RM-001",
            UOM = "KG",
            AvailableQuantity = 500000
        },
        new RawMaterialDto
        {
            Id = Guid.Parse("bbbb2222-2222-2222-2222-222222222222"),
            Name = "Copper Wire",
            SKU = "RM-002",
            UOM = "MTR",
            AvailableQuantity = 3000000
        }
    };
}
