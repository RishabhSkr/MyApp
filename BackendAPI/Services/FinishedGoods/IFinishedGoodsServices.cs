using BackendAPI.Dtos.Inventory;

namespace BackendAPI.Services.FinishedGoods
{
    public interface IFinishedGoodsService
    {
        Task<IEnumerable<FinishedGoodsResponseDto>> GetCurrentStockAsync();
    }
}