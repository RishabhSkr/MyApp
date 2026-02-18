using BackendAPI.Dtos.Bom;
using BackendAPI.Models;
using BomEntity = BackendAPI.Models.Bom;

namespace BackendAPI.Repositories.BomRepository
{
    public interface IBomRepository
    {
        Task<IEnumerable<BomEntity>> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<BomEntity>> GetAllAsync();
        Task<bool>ExistsAsync(Guid productId); // product ka bom exist karta hain
        Task AddRangeAsync(IEnumerable<BomEntity> boms); // BULK Insert
        Task DeleteByProductIdAsync(Guid productId); // Purana delete karke naya dalne ke liye
    }
}
