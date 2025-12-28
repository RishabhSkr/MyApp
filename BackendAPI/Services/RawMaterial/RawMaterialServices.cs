using System.Security.Cryptography.X509Certificates;
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

        
        public RawMaterialService(IRawMaterialRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<IEnumerable<RawMaterialResponseDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();

            // Mapping Entity -> DTO
            return entities.Select(e => new RawMaterialResponseDto
            {
                Id = e.RawMaterialId,
                Name = e.Name,
                SKU = e.SKU,
                UOM = e.UOM,
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
                    UOM = dto.UOM,
                    CreatedByUserId = userId
                };

                // Repo me AddAsync sirf RM save kar raha hai, 
                // lekin hume ID chahiye Inventory ke liye.
                // Isliye hum Context directly use karenge ya Repo ko modify karenge.
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

        public async Task<string> UpdateRawMaterialAsync(int id,RawMaterialUpdateDto dto, int userId)
        {   
            var material = await _repo.GetByIdAsync(id);

            if (material == null)
                throw new NotFoundException("Raw Material not found.");

            // duplicate check
            var alreadyNameExists = await _repo.ExistsBySkuAsync(dto.SKU);
            if (material.SKU != dto.SKU && alreadyNameExists)
                throw new BadRequestException("Raw Material SKU already taken.");

            // Mapping DTO -> Entity
            material.Name = dto.Name;
            material.SKU = dto.SKU;
            material.UOM = dto.UOM;

            // Audit
            material.UpdatedByUserId = userId;

            await _repo.UpdateRawMaterialAsync(material);

            return "Success";
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

        //    public async Task<RawMaterialResponseDto> GetByIdAsync(int id)
        // {
        //     var entity = await _repo.GetByIdAsync(id);

        //     if (entity == null)
        //         throw new NotFoundException("Raw Material not found.");

        //     // Mapping Entity -> DTO
        //     return new RawMaterialResponseDto
        //     {
        //         Id = entity.RawMaterialId,
        //         Name = entity.Name,
        //         SKU = entity.SKU,
        //         CreatedByUserId = entity.CreatedByUserId,
        //         CreatedAt = entity.CreatedAt,
        //         UpdatedByUserId = entity.UpdatedByUserId,
        //         UpdatedAt = entity.UpdatedAt,

        //         // Inventory Mapping
        //         Inventories = entity.Inventory != null 
        //             ? new List<RawMaterialInventoryResponseDto> 
        //             { 
        //                 new RawMaterialInventoryResponseDto 
        //                 { 
        //                     InventoryId = entity.Inventory.Id,
        //                     AvailableQuantity = entity.Inventory.AvailableQuantity 
        //                 } 
        //             }
        //             : new List<RawMaterialInventoryResponseDto>()
        //     };
        // }
        
    }
}