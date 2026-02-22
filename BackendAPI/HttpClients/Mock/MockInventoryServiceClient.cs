using BackendAPI.HttpClients.Dtos;

namespace BackendAPI.HttpClients.Mock;

/// <summary>
/// Mock Inventory Service — actually modifies MockDataStore data
/// Reserve → qty deduct, Return → qty add back, FG → stock add
/// </summary>
public class MockInventoryServiceClient : IInventoryServiceClient
{
    public Task<ProductDto?> GetProductAsync(Guid productId)
    {
        var product = MockDataStore.Products.FirstOrDefault(p => p.Id == productId);
        return Task.FromResult(product);
    }

    public Task<List<RawMaterialDto>> GetRawMaterialsByIdsAsync(List<Guid> ids)
    {
        var result = MockDataStore.RawMaterials.Where(r => ids.Contains(r.Id)).ToList();
        return Task.FromResult(result);
    }

    // RESERVE → AvailableQuantity DEDUCT karo
    public Task<bool> ReserveMaterialsAsync(InventoryReservationRequest request)
    {
        foreach (var item in request.Items)
        {
            var rm = MockDataStore.RawMaterials.FirstOrDefault(r => r.Id == item.RawMaterialId);
            if (rm != null)
            {
                rm.AvailableQuantity -= item.Quantity;
                Console.WriteLine($"[MOCK] Reserved {item.Quantity} of {rm.Name} → Remaining: {rm.AvailableQuantity}");
            }
        }
        return Task.FromResult(true);
    }

    // RETURN → AvailableQuantity WAPAS ADD karo
    public Task<bool> ReturnMaterialsAsync(InventoryReservationRequest request)
    {
        foreach (var item in request.Items)
        {
            var rm = MockDataStore.RawMaterials.FirstOrDefault(r => r.Id == item.RawMaterialId);
            if (rm != null)
            {
                rm.AvailableQuantity += item.Quantity;
                Console.WriteLine($"[MOCK] Returned {item.Quantity} of {rm.Name} → Available: {rm.AvailableQuantity}");
            }
        }
        return Task.FromResult(true);
    }

    // FINISHED GOODS → Product stock me add karo
    public Task<bool> AddFinishedGoodsAsync(FinishedGoodsRequest request)
    {
        var product = MockDataStore.Products.FirstOrDefault(p => p.Id == request.ProductId);
        if (product != null)
        {
            Console.WriteLine($"[MOCK] Added {request.ProducedQuantity} finished '{product.Name}' (Scrap: {request.ScrapQuantity})");
        }
        return Task.FromResult(true);
    }
}
