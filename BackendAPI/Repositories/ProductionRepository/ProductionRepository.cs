using BackendAPI.Data;
using BackendAPI.Models;
using Microsoft.EntityFrameworkCore;
using ProdOrder = BackendAPI.Models.ProductionOrder;

namespace BackendAPI.Repositories.ProductionRepository
{
    public class ProductionRepository : IProductionRepository
    {
        private readonly AppDbContext _context;

        public ProductionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProdOrder> AddAsync(ProdOrder order)
        {
            _context.ProductionOrders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<IEnumerable<ProdOrder>> GetAllAsync()
        {
            return await _context.ProductionOrders
                                 .Include(p => p.Product)
                                 .Include(p => p.SalesOrder)
                                 .Include(p => p.CreatedByUser)
                                 .Where(p => p.IsActive)
                                 .OrderByDescending(p => p.CreatedAt)
                                 .ToListAsync();
        }

        public async Task<bool> ExistsBySalesOrderIdAsync(int salesOrderId)
        {
            // Check agar koi active production order is sales order se link hai
            return await _context.ProductionOrders
                                 .AnyAsync(p => p.SalesOrderId == salesOrderId && p.IsActive);
        }
    }
}