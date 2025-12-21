using BackendAPI.Data;
using BackendAPI.Models;
using Microsoft.EntityFrameworkCore;
using RawMaterialEntity = BackendAPI.Models.RawMaterial;

namespace BackendAPI.Repositories.RawMaterial
{
    public class RawMaterialRepository : IRawMaterialRepository
    {
        private readonly AppDbContext _context;

        public RawMaterialRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RawMaterialEntity>> GetAllAsync()
        {
            return await _context.RawMaterials
                                .Include(r => r.Inventory) // Inventory data fetch karne ke liye
                                .Where(r => r.IsActive)    // Sirf Active items
                                .OrderByDescending(r => r.CreatedAt) // Latest upar
                                .ToListAsync();
        }
        
        public async Task<RawMaterialEntity?> GetByIdAsync(int id)
        {
            return await _context.RawMaterials
                                 .FirstOrDefaultAsync(r => r.RawMaterialId == id && r.IsActive);
        }

        public async Task<RawMaterialEntity?> GetBySkuAsync(string sku)
        {
            return await _context.RawMaterials
                                 .FirstOrDefaultAsync(r => r.SKU == sku && r.IsActive);
        }

        public async Task<RawMaterialInventory?> GetInventoryByMaterialIdAsync(int rawMaterialId)
        {
            return await _context.RawMaterialInventories
                                 .FirstOrDefaultAsync(i => i.RawMaterialId == rawMaterialId && i.IsActive);
        }

        public async Task AddAsync(RawMaterialEntity rawMaterial)
        {
            _context.RawMaterials.Add(rawMaterial);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInventoryAsync(RawMaterialInventory inventory)
        {
            _context.RawMaterialInventories.Update(inventory);
            await _context.SaveChangesAsync();
        }
    }
}