using BackendAPI.Data;
using BackendAPI.Dtos.Production;
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

        public async Task<decimal> GetTotalPlannedQtyBySalesOrderIdAsync(Guid salesOrderId)
        {
            // return await _context.ProductionOrders
            //                      .Where(p => p.SalesOrderId == salesOrderId) 
            //                      //  count (Completed + Planned + InProgress)
            //                      .SumAsync(p => p.PlannedQuantity);

            var orders = await _context.ProductionOrders
                               .Where(p => p.SalesOrderId == salesOrderId)
                               .Where(p => p.Status != "Cancelled")
                               .ToListAsync();

            // Hybrid Sum: Consistent formula
            // Completed → ProducedQty (actual output — scrap/unused excluded)
            // Active (Planned/Released/InProgress) → PlannedQty (target — blocks room)
            // This naturally handles:
            //   - Over-planning prevention: active batches block room via PlannedQty
            //   - Shortfall re-planning: completed batch scrap opens room via ProducedQty < PlannedQty
            return orders.Sum(p => 
                p.Status == "Completed" ? p.ProducedQuantity : p.PlannedQuantity);
        }
        public async Task<IEnumerable<ProdOrder>> GetAllAsync()
        {
            return await _context.ProductionOrders
                                 .Where(p => p.IsActive)
                                 .OrderByDescending(p => p.CreatedAt)
                                 .ToListAsync();
        }
        public async Task<bool> ExistsBySalesOrderIdAsync(Guid salesOrderId)
        {
            // Check agar koi active production order is sales order se link hai
            return await _context.ProductionOrders
                                 .AnyAsync(p => p.SalesOrderId == salesOrderId && p.IsActive);
        }

        public async Task<IEnumerable<PendingOrderDto>> GetPendingSalesOrdersAsync()
        {
            return await _context.ProductionOrders
                                 .Where(p => p.IsActive)
                                 .Select(p => new PendingOrderDto
                                 {
                                     // Map properties from ProdOrder to PendingOrderDto
                                 })
                                 .ToListAsync();
        }
    }
}