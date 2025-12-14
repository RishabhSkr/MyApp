using BackendAPI.Models;

namespace BackendAPI.Repositories.Bom
{
    public interface IBomRepository
    {
        Task<IEnumerable<Models.Bom>> GetByProductIdAsync(int productId);
        Task<bool>ExistsAsync(int productId); // product ka bom exist karta hain
        Task AddRangeAsync(IEnumerable<Models.Bom> boms); // BULK Insert
        Task DeleteByProductIdAsync(int productId); // Purana delete karke naya dalne ke liye
    }
}
