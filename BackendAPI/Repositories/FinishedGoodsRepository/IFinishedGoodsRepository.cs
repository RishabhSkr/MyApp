using BackendAPI.Models;

namespace BackendAPI.Repositories.FinishedGoodsRepository
{
    public interface IFinishedGoodsRepository
    {
        Task<IEnumerable<FinishedGoodsInventory>> GetAllAsync();
        Task<FinishedGoodsInventory?> GetByProductIdAsync(int productId);
    }
}