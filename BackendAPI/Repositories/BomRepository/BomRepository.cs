using BackendAPI.Data;
using BackendAPI.Models;
using Microsoft.EntityFrameworkCore;
using BomEntity = BackendAPI.Models.Bom;


namespace BackendAPI.Repositories.BomRepository
{
    public class BomRepository : IBomRepository
    {
        private readonly AppDbContext _context;

        public BomRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<BomEntity>> GetAllAsync()
        {
            // Humein Product aur RawMaterial dono ka data chahiye
            return await _context.BOMs
                                .Where(b => b.IsActive)       // Sirf Active Records
                                .ToListAsync();
        }
        public async Task<bool> ExistsAsync(Guid productId)
        {
            return await _context.BOMs.AnyAsync(b => b.ProductId == productId && b.IsActive);
        }

       public async Task<IEnumerable<BomEntity>> GetByProductIdAsync(Guid productId)
        {
            return await _context.BOMs
                        .Where(b => b.ProductId == productId && b.IsActive)
                        .ToListAsync();
        }
        public async Task AddRangeAsync(IEnumerable<BomEntity> boms)
        {
            await _context.BOMs.AddRangeAsync(boms);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByProductIdAsync(Guid productId)
        {
            // Soft Delete Logic:
            // Purani recipe ko 'IsActive = false' mark karo
            var oldBoms = await _context.BOMs
                                        .Where(b => b.ProductId == productId && b.IsActive)
                                        .ToListAsync();

            if (oldBoms.Any())
            {
                foreach (var item in oldBoms)
                {
                    item.IsActive = false;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}