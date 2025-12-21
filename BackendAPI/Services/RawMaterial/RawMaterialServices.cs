using BackendAPI.Data;
using BackendAPI.Dtos.RawMaterial;
using BackendAPI.Exceptions;
using BackendAPI.Models;
using BackendAPI.Repositories.RawMaterial;
using RawMaterialEntity = BackendAPI.Models.RawMaterial;

namespace BackendAPI.Services.RawMaterial
{
    public class RawMaterialService : IRawMaterialService
    {
        private readonly IRawMaterialRepository _repo;
        private readonly AppDbContext _context; // For Transaction

        public async Task<IEnumerable<RawMaterialResponseDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();

            // Mapping Entity -> DTO
            return entities.Select(e => new RawMaterialResponseDto
            {
                Id = e.RawMaterialId,
                Name = e.Name,
                SKU = e.SKU,
                CreatedByUserId = e.CreatedByUserId,
                CreatedAt = e.CreatedAt,
                UpdatedByUserId = e.UpdatedByUserId,
                UpdatedAt = e.UpdatedAt,

                // Inventory Mapping
                // Kyunki DB mein 1:1 relation hai, lekin DTO mein List hai, hum list mein add kar rahe hain
                Inventories = e.Inventory != null 
                    ? new List<RawMaterialInventoryResponseDto> 
                    { 
                        new RawMaterialInventoryResponseDto 
                        { 
                            InventoryId = e.Inventory.Id,
                            AvailableQuantity = e.Inventory.AvailableQuantity 
                        } 
                    }
                    : new List<RawMaterialInventoryResponseDto>() // Agar inventory nahi bani to empty list
            }).ToList();
        }
        public RawMaterialService(IRawMaterialRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        //  CREATE NEW MATERIAL (With Empty Inventory)
        public async Task<string> CreateRawMaterialAsync(RawMaterialCreateDto dto, int userId)
        {
            // Validation
            if (await _repo.GetBySkuAsync(dto.SKU) != null)
                return "Error: SKU already exists.";

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // A. Create Material
                var newMaterial = new RawMaterialEntity
                {
                    Name = dto.Name,
                    SKU = dto.SKU,
                    CreatedByUserId = userId
                };

                // Repo me AddAsync sirf RM save kar raha hai, 
                // lekin hume ID chahiye Inventory ke liye.
                // Isliye hum Context directly use karenge ya Repo ko modify karenge.
                // Best practice: Let's use Repo but ensure SaveChanges calls are managed.
                // Simplify: Adding directly to context here for transaction safety
                
                _context.RawMaterials.Add(newMaterial);
                await _context.SaveChangesAsync(); // Generates ID

                // B. Create Empty Inventory Record
                var newInventory = new RawMaterialInventory
                {
                    RawMaterialId = newMaterial.RawMaterialId, // Generated ID
                    AvailableQuantity = 0, // Shuru me 0
                    CreatedByUserId = userId,
                    UpdatedByUserId = userId
                };

                _context.RawMaterialInventories.Add(newInventory);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Error: {ex.Message}";
            }
        }

        // ADD STOCK (Purchase)
        public async Task<string> AddStockAsync(StockUpdateDto dto, int userId)
        {
            var inventory = await _repo.GetInventoryByMaterialIdAsync(dto.RawMaterialId);

            if (inventory == null)
                throw new NotFoundException("Inventory record not found for this material.");

            // Logic: Existing + New
            inventory.AvailableQuantity += dto.Quantity;
            
            // Audit
            inventory.UpdatedByUserId = userId;

            await _repo.UpdateInventoryAsync(inventory);
            return "Success";
        }
    }
}