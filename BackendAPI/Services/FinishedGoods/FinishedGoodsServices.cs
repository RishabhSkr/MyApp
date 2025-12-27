using BackendAPI.Dtos.Inventory;
using BackendAPI.Repositories.FinishedGoodsRepository;

namespace BackendAPI.Services.FinishedGoods
{
    public class FinishedGoodsService : IFinishedGoodsService
    {
        private readonly IFinishedGoodsRepository _repo;

        public FinishedGoodsService(IFinishedGoodsRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<FinishedGoodsResponseDto>> GetCurrentStockAsync()
        {
            var inventory = await _repo.GetAllAsync();

            return inventory.Select(i => new FinishedGoodsResponseDto
            {
                InventoryId = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "Unknown Product",
                AvailableQuantity = i.AvailableQuantity,
                CreatedAt = i.CreatedAt,
                LastUpdated = i.UpdatedAt ?? i.CreatedAt
            }).ToList();
        }
    }
}