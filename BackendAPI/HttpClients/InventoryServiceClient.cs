using System.Net.Http.Json;
using BackendAPI.HttpClients.Dtos;

namespace BackendAPI.HttpClients;

public class InventoryServiceClient : IInventoryServiceClient
{
    private readonly HttpClient _http;

    public InventoryServiceClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ProductDto?> GetProductAsync(Guid productId)
    {
        return await _http.GetFromJsonAsync<ProductDto>($"api/products/{productId}");
    }

    public async Task<List<RawMaterialDto>> GetRawMaterialsByIdsAsync(List<Guid> ids)
    {
        var response = await _http.PostAsJsonAsync("api/raw-materials/by-ids", ids);
        return await response.Content.ReadFromJsonAsync<List<RawMaterialDto>>() ?? new();
    }

    public async Task<bool> ReserveMaterialsAsync(InventoryReservationRequest request)
    {
        var response = await _http.PutAsJsonAsync("api/inventory/raw-material/reserve", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ReturnMaterialsAsync(InventoryReservationRequest request)
    {
        var response = await _http.PutAsJsonAsync("api/inventory/raw-material/return", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AddFinishedGoodsAsync(FinishedGoodsRequest request)
    {
        var response = await _http.PutAsJsonAsync("api/inventory/finished-goods/add", request);
        return response.IsSuccessStatusCode;
    }
}