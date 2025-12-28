using SalesEntity = BackendAPI.Models.SalesOrder;

namespace BackendAPI.Repositories.SalesRepository
{
    public interface ISalesRepository
    {
        Task<IEnumerable<SalesEntity>> GetAllAsync();
        Task<SalesEntity?> GetByIdAsync(int id);
        
        Task<IEnumerable<SalesEntity>> GetOrdersByStatusAsync(string status);
    }
}