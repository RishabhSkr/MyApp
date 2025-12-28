using BackendAPI.Data;
using Microsoft.EntityFrameworkCore;
using SalesEntity = BackendAPI.Models.SalesOrder;

namespace BackendAPI.Repositories.SalesRepository
{
    public class SalesRepository : ISalesRepository
    {
        private readonly AppDbContext _context;
        public SalesRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<SalesEntity>> GetAllAsync()
        {
            return await _context.SalesOrders
                                            .Include(p => p.Product)
                                            .Where(p => p.IsActive)
                                            .ToListAsync();
        }
        public async Task<SalesEntity?> GetByIdAsync(int id)                                      
        {
           return await _context.SalesOrders
                            .Include(p => p.CreatedByUser)
                            .Include(p => p.Product) 
                            .FirstOrDefaultAsync(p => p.SalesOrderId == id && p.IsActive);
        }
        // SQL Filtering
        public async Task<IEnumerable<SalesEntity>> GetOrdersByStatusAsync(string status)
        {
            return await _context.SalesOrders
                                 .Include(s => s.Product) 
                                 .Include(s => s.CreatedByUser) 
                                 .Where(s => s.Status == status && s.IsActive) 
                                 .OrderByDescending(s => s.OrderDate) 
                                 .ToListAsync();
        }
    }
}