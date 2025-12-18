using BackendAPI.Models;
// Alias to avoid conflict
using ProdOrder = BackendAPI.Models.ProductionOrder;

namespace BackendAPI.Repositories.ProductionRepository
{
    public interface IProductionRepository
    {
        Task<ProdOrder> AddAsync(ProdOrder order);
        Task<IEnumerable<ProdOrder>> GetAllAsync();
        
        // Validation ke liye: Kya is SalesOrder par pehle se Production chal rahi hai?
        Task<bool> ExistsBySalesOrderIdAsync(int salesOrderId);
    }
}