using BackendAPI.HttpClients.Dtos;

namespace BackendAPI.HttpClients;

public interface ISalesServiceClient
{
    Task<SalesOrderDto?> GetSalesOrderAsync(Guid salesOrderId);
    Task<List<SalesOrderDto>> GetPendingSalesOrdersAsync();
    Task<bool> UpdateSalesOrderStatusAsync(Guid salesOrderId, string status);
}