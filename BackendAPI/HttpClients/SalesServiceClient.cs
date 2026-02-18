using System.Net.Http.Json;
using BackendAPI.HttpClients.Dtos;

namespace BackendAPI.HttpClients;

public class SalesServiceClient : ISalesServiceClient
{
    private readonly HttpClient _http;

    public SalesServiceClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<SalesOrderDto?> GetSalesOrderAsync(Guid salesOrderId)
    {
        return await _http.GetFromJsonAsync<SalesOrderDto>($"api/sales/{salesOrderId}");
    }

    public async Task<List<SalesOrderDto>> GetPendingSalesOrdersAsync()
    {
        return await _http.GetFromJsonAsync<List<SalesOrderDto>>("api/sales/pending") ?? new();
    }

    public async Task<bool> UpdateSalesOrderStatusAsync(Guid salesOrderId, string status)
    {
        var response = await _http.PutAsJsonAsync($"api/sales/{salesOrderId}/status", new { status });
        return response.IsSuccessStatusCode;
    }
}