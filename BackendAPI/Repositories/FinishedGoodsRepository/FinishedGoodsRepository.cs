using BackendAPI.Data;
using BackendAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Repositories.FinishedGoodsRepository
{
    public class FinishedGoodsRepository : IFinishedGoodsRepository
    {
        private readonly AppDbContext _context;

        public FinishedGoodsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FinishedGoodsInventory>> GetAllAsync()
        {
            return await _context.FinishedGoodsInventories
                                 .Include(f => f.Product) // Product details fetch karein
                                 .Where(f => f.IsActive)  
                                //  .OrderBy(f => f.Product!.Name)
                                 .ToListAsync();
        }

        public async Task<FinishedGoodsInventory?> GetByProductIdAsync(int productId)
        {
            return await _context.FinishedGoodsInventories
                                 .Include(f => f.Product)
                                 .FirstOrDefaultAsync(f => f.ProductId == productId && f.IsActive);
        }
    }
}