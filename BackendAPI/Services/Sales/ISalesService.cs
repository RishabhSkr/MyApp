
using BackendAPI.Dtos.Sales;

namespace BackendAPI.Services.Sales
{
    public interface ISalesService
    {
        Task<IEnumerable<SalesResponseDto>> GetPendingSalesOrdersAsync();
    }
}