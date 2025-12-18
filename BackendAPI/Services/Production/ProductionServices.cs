using BackendAPI.Data;
using BackendAPI.Dtos.Production;
using BackendAPI.Repositories.ProductionRepository;
using Microsoft.EntityFrameworkCore;
using ProdOrder = BackendAPI.Models.ProductionOrder;

namespace BackendAPI.Services.Production
{
    public class ProductionService : IProductionService
    {
        private readonly IProductionRepository _repo;
        private readonly AppDbContext _context; // Direct access for complex checks

        public ProductionService(IProductionRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<string> CreateProductionPlanAsync(CreateProductionDto dto, int userId)
        {
            // 1. Validate: Sales Order Exist karta hai?
            var salesOrder = await _context.SalesOrders
                                           .Include(s => s.Product)
                                           .FirstOrDefaultAsync(s => s.SalesOrderId == dto.SalesOrderId);

            if (salesOrder == null) return "Error: Sales Order not found.";

            // 2. Validate: Kya iska Production pehle hi ban chuka hai?
            if (await _repo.ExistsBySalesOrderIdAsync(dto.SalesOrderId))
                return "Error: Production Order already exists for this Sales Order.";

            // 3. FETCH BOM & INVENTORY (Recipe + Stock)
            var bomList = await _context.BOMs
                                        .Include(b => b.RawMaterial)
                                        .ThenInclude(rm => rm.Inventory) // <-- Inventory tak pahunche
                                        .Where(b => b.ProductId == salesOrder.ProductId && b.IsActive)
                                        .ToListAsync();

            if (!bomList.Any())
                return $"Error: No BOM (Recipe) found for product '{salesOrder.Product?.Name}'. Cannot manufacture.";

            // 4. STOCK CHECK
            foreach (var item in bomList)
            {
                // Formula: 1 Unit ke liye * Total Order Quantity
                decimal totalRequired = item.QuantityRequired * salesOrder.Quantity;
                
                // Available Stock (Null check zaroori hai)
                decimal availableStock = item.RawMaterial?.Inventory?.AvailableQuantity ?? 0;

                if (availableStock < totalRequired)
                {
                    return $"STOCK ALERT: Not enough {item.RawMaterial?.Name}. Required: {totalRequired}, Available: {availableStock}";
                }
            }

            // 5. SAB THEEK HAI -> CREATE ORDER
            var prodOrder = new ProdOrder
            {
                SalesOrderId = salesOrder.SalesOrderId,
                ProductId = salesOrder.ProductId,
                PlannedQuantity = salesOrder.Quantity,
                ProducedQuantity = 0,
                Status = "Planned", // Abhi sirf Plan bana hai, material kata nahi hai
                CreatedByUserId = userId,
                StartDate = null
            };

            await _repo.AddAsync(prodOrder);

            // Optional: Sales Order status update kar sakte hain
            salesOrder.Status = "In Production";
            await _context.SaveChangesAsync();

            return "Success";
        }
    }
}