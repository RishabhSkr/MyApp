using BackendAPI.HttpClients.Dtos;

namespace BackendAPI.HttpClients;

public interface IInventoryServiceClient
{
    Task<ProductDto?> GetProductAsync(Guid productId);
    Task<List<RawMaterialDto>> GetRawMaterialsByIdsAsync(List<Guid> ids);
    Task<bool> ReserveMaterialsAsync(InventoryReservationRequest request);
    Task<bool> ReturnMaterialsAsync(InventoryReservationRequest request);
    Task<bool> AddFinishedGoodsAsync(FinishedGoodsRequest request);
}