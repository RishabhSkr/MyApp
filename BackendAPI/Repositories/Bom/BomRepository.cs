using BackendAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Repositories.Bom
{
    public class BomRepository : IBomRepository
    {
        private readonly AppDbContext _context;

        public BomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.Bom>> GetByProductIdAsync(int productId)
        {
            return await _context.BOMs
                .Include(b => b.Product)
                .Include(b => b.RawMaterial)
                .Where(b => b.ProductID == productId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int productId)
        {
            return await _context.BOMs.AnyAsync(b => b.ProductID == productId);
        }

        // 🚀 FAST: Ek saath save karega
        public async Task AddRangeAsync(IEnumerable<Models.Bom> boms)
        {
            await _context.BOMs.AddRangeAsync(boms);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByProductIdAsync(int productId)
        {
            var boms = await _context.BOMs.Where(b => b.ProductID == productId).ToListAsync();
            _context.BOMs.RemoveRange(boms);
            await _context.SaveChangesAsync();
        }
    }
}