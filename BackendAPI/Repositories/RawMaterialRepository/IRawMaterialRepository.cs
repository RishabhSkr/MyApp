using BackendAPI.Models;
// Alias to avoid conflict
using RawMaterialEntity = BackendAPI.Models.RawMaterial;

namespace BackendAPI.Repositories.RawMaterial
{
    public interface IRawMaterialRepository
    {
        Task<RawMaterialEntity?> GetByIdAsync(int id);
        Task<RawMaterialEntity?> GetBySkuAsync(string sku);
        Task<IEnumerable<RawMaterialEntity>> GetAllAsync();
        
        // Inventory ke sath fetch karne ke liye
        Task<RawMaterialInventory?> GetInventoryByMaterialIdAsync(int rawMaterialId);
        
        Task AddAsync(RawMaterialEntity rawMaterial);
        Task UpdateInventoryAsync(RawMaterialInventory inventory);
    }
}