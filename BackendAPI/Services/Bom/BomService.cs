using BackendAPI.Data;
using BackendAPI.Dtos.Bom;
using BackendAPI.Repositories.BomRepository;

// Alias
using BomEntity = BackendAPI.Models.Bom; 

namespace BackendAPI.Services.Bom
{
    public class BomService : IBomService
    {
        private readonly IBomRepository _repo;
        private readonly AppDbContext _context; // Transaction ke liye direct access chahiye

        public BomService(IBomRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }
        // GET ALL BOMS (Grouping Logic)
        public async Task<IEnumerable<BomResponseDto>> GetAllBomsAsync()
        {
            var allBoms = await _repo.GetAllAsync();

            // Database "Flat List" deta hai (Row by Row).
            // Humein usse "Product ke hisaab se Group" karna hai.
            var groupedResponse = allBoms
                .GroupBy(b => b.ProductId)
                .Select(g => new BomResponseDto
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product?.Name ?? "Unknown",
                    CreatedAt = g.First().CreatedAt,
                    UpdatedAt = g.First().UpdatedAt,
                    CreatedByUserId = g.First().CreatedByUserId,
                    UpdatedByUserId = g.First().UpdatedByUserId,
                    Materials = g.Select(b => new BomItemDto
                    {
                        RawMaterialId = b.RawMaterialId,
                        RawMaterialName = b.RawMaterial?.Name ?? "Unknown",
                        SKU = b.RawMaterial?.SKU ?? "",
                        QuantityRequired = b.QuantityRequired,
                        UOM = b.RawMaterial?.UOM ?? ""

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
            Console.WriteLine(boms);
            // Manual Mapping to DTO
            return new BomResponseDto
            {
                ProductId = productId,
                ProductName = boms.First().Product?.Name ?? "Unknown",
                CreatedAt = boms.First().CreatedAt,
                UpdatedAt = boms.First().UpdatedAt,
                CreatedByUserId = boms.First().CreatedByUserId,
                UpdatedByUserId = boms.First().UpdatedByUserId,
                Materials = boms.Select(b => new BomItemDto
                {
                    RawMaterialId = b.RawMaterialId,
                    RawMaterialName = b.RawMaterial?.Name ?? "",
                    SKU = b.RawMaterial?.SKU ?? "",
                    QuantityRequired = b.QuantityRequired,
                    UOM = b.RawMaterial?.UOM ?? ""
                }).ToList()
            };
        }

        // CREATE BOM
        public async Task<string> CreateBomAsync(BomCreateDto dto, Guid userId)
        {
            // Validation: Kya pehle se BOM hai?
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

        // 2. UPDATE BOM (Transaction: Soft Delete Old + Insert New)
        public async Task<string> UpdateBomAsync(Guid productId, BomCreateDto dto, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (dto.ProductId != productId) return "Error: Product ID mismatch.";

                // A. Purane BOM ko 'Delete' (Soft Delete) karo
                await _repo.DeleteByProductIdAsync(productId);

                // B. Naya BOM List taiyaar karo
                var newBoms = new List<BomEntity>();
                foreach (var item in dto.BomItems)
                {
                    newBoms.Add(new BomEntity
                    {
                        ProductId = productId,
                        RawMaterialId = item.RawMaterialId,
                        QuantityRequired = item.QuantityRequired,
                        CreatedByUserId = userId,
                        UpdatedByUserId = userId // Update ke waqt dono ID same rakh sakte hain ya alag logic
                    });
                }

                // C. Save New
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
                // Check karo exist karta hai ya nahi
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
