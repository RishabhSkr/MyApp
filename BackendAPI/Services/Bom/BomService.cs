using BackendAPI.Data;
using BackendAPI.Dtos.Bom;
using BackendAPI.HttpClients;
using BackendAPI.Repositories.BomRepository;

// Alias
using BomEntity = BackendAPI.Models.Bom; 

namespace BackendAPI.Services.Bom
{
    public class BomService : IBomService
    {
        private readonly IBomRepository _repo;
        private readonly AppDbContext _context;
        private readonly IInventoryServiceClient _inventoryService;

        public BomService(IBomRepository repo, AppDbContext context, IInventoryServiceClient inventoryService)
        {
            _repo = repo;
            _context = context;
            _inventoryService = inventoryService;
        }

        // GET ALL BOMS (Grouping Logic)
        public async Task<IEnumerable<BomResponseDto>> GetAllBomsAsync()
        {
            var allBoms = await _repo.GetAllAsync();

            // Unique IDs collect karo for batch fetch
            var productIds = allBoms.Select(b => b.ProductId).Distinct().ToList();
            var rmIds = allBoms.Select(b => b.RawMaterialId).Distinct().ToList();

            // HTTP batch fetch — ek baar me sab laao
            var products = new Dictionary<Guid, string>();
            foreach (var pid in productIds)
            {
                var p = await _inventoryService.GetProductAsync(pid);
                if (p != null) products[pid] = p.Name;
            }

            var rawMaterials = await _inventoryService.GetRawMaterialsByIdsAsync(rmIds);
            var rmLookup = rawMaterials.ToDictionary(r => r.Id);

            // Group by Product
            var groupedResponse = allBoms
                .GroupBy(b => b.ProductId)
                .Select(g => new BomResponseDto
                {
                    ProductId = g.Key,
                    ProductName = products.GetValueOrDefault(g.Key, "Unknown"),
                    CreatedAt = g.First().CreatedAt,
                    UpdatedAt = g.First().UpdatedAt,
                    CreatedByUserId = g.First().CreatedByUserId,
                    UpdatedByUserId = g.First().UpdatedByUserId,
                    Materials = g.Select(b =>
                    {
                        rmLookup.TryGetValue(b.RawMaterialId, out var rm);
                        return new BomItemDto
                        {
                            RawMaterialId = b.RawMaterialId,
                            RawMaterialName = rm?.Name ?? "Unknown",
                            SKU = rm?.SKU ?? "",
                            QuantityRequired = b.QuantityRequired,
                            UOM = rm?.UOM ?? ""
                        };
                    }).ToList()
                })
                .ToList();

            return groupedResponse;
        }

        // GET BOM BY ProductId
        public async Task<BomResponseDto?> GetBomByProductAsync(Guid productId)
        {
            var boms = await _repo.GetByProductIdAsync(productId);
            if (!boms.Any()) return null;

            // Fetch product name via HTTP
            var product = await _inventoryService.GetProductAsync(productId);

            // Fetch raw material names via HTTP
            var rmIds = boms.Select(b => b.RawMaterialId).Distinct().ToList();
            var rawMaterials = await _inventoryService.GetRawMaterialsByIdsAsync(rmIds);
            var rmLookup = rawMaterials.ToDictionary(r => r.Id);

            return new BomResponseDto
            {
                ProductId = productId,
                ProductName = product?.Name ?? "Unknown",
                CreatedAt = boms.First().CreatedAt,
                UpdatedAt = boms.First().UpdatedAt,
                CreatedByUserId = boms.First().CreatedByUserId,
                UpdatedByUserId = boms.First().UpdatedByUserId,
                Materials = boms.Select(b =>
                {
                    rmLookup.TryGetValue(b.RawMaterialId, out var rm);
                    return new BomItemDto
                    {
                        RawMaterialId = b.RawMaterialId,
                        RawMaterialName = rm?.Name ?? "",
                        SKU = rm?.SKU ?? "",
                        QuantityRequired = b.QuantityRequired,
                        UOM = rm?.UOM ?? ""
                    };
                }).ToList()
            };
        }

        // CREATE BOM
        public async Task<string> CreateBomAsync(BomCreateDto dto, Guid userId)
        {
            if (await _repo.ExistsAsync(dto.ProductId))
                return "Error: BOM already exists for this product. Use Update instead.";

            var bomList = new List<BomEntity>();

            foreach (var item in dto.BomItems)
            {
                bomList.Add(new BomEntity
                {
                    ProductId = dto.ProductId,
                    RawMaterialId = item.RawMaterialId,
                    QuantityRequired = item.QuantityRequired,
                    CreatedByUserId = userId 
                });
            }

            await _repo.AddRangeAsync(bomList);
            return "Success";
        }

        // UPDATE BOM (Transaction: Soft Delete Old + Insert New)
        public async Task<string> UpdateBomAsync(Guid productId, BomCreateDto dto, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (dto.ProductId != productId) return "Error: Product ID mismatch.";

                await _repo.DeleteByProductIdAsync(productId);

                var newBoms = new List<BomEntity>();
                foreach (var item in dto.BomItems)
                {
                    newBoms.Add(new BomEntity
                    {
                        ProductId = productId,
                        RawMaterialId = item.RawMaterialId,
                        QuantityRequired = item.QuantityRequired,
                        CreatedByUserId = userId,
                        UpdatedByUserId = userId
                    });
                }

                await _repo.AddRangeAsync(newBoms);

                await transaction.CommitAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> DeleteBomAsync(Guid productId)
        {
            try{
                if (!await _repo.ExistsAsync(productId))
                    return "Error: BOM not found.";

                await _repo.DeleteByProductIdAsync(productId);
                return "Success";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
