using BackendAPI.HttpClients.Dtos;
using BackendAPI.Constants;
namespace BackendAPI.HttpClients.Mock;

/// <summary>
/// Mock Sales Service — uses shared MockDataStore
/// </summary>
public class MockSalesServiceClient : ISalesServiceClient
{
    public Task<SalesOrderDto?> GetSalesOrderAsync(Guid salesOrderId)
    {
        var order = MockDataStore.SalesOrders.FirstOrDefault(o => o.Id == salesOrderId);
        return Task.FromResult(order);
    }

    public Task<List<SalesOrderDto>> GetPendingSalesOrdersAsync()
    {
        // var pending = MockDataStore.SalesOrders.Where(o => o.Status == "Pending").ToList();
        // pending = all orders except completed and cancelled
        var pending = MockDataStore.SalesOrders
        .Where(o => o.Status != EventStatus.COMPLETED && o.Status != EventStatus.CANCELLED)
        .ToList();
        return Task.FromResult(pending);
    }

    public Task<bool> UpdateSalesOrderStatusAsync(Guid salesOrderId, string status)
    {
        var order = MockDataStore.SalesOrders.FirstOrDefault(o => o.Id == salesOrderId);
        if (order != null) order.Status = status;
        Console.WriteLine($"[MOCK] Sales Order {salesOrderId} status → {status}");
        return Task.FromResult(true);
    }
}
