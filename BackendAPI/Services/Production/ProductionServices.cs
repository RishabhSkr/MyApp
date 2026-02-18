using BackendAPI.Data;
using BackendAPI.Dtos.Production;
using BackendAPI.Repositories.ProductionRepository;
using Microsoft.EntityFrameworkCore;
using ProdOrder = BackendAPI.Models.ProductionOrder;
using BackendAPI.HttpClients;
using BackendAPI.HttpClients.Dtos;
using BackendAPI.Utilities;

namespace BackendAPI.Services.Production
{
    public class ProductionService : IProductionService
    {
        private readonly IProductionRepository _repo;
        private readonly AppDbContext _context; 
        private readonly IInventoryServiceClient _inventoryService;
        private readonly ISalesServiceClient _salesService;

        public ProductionService(IProductionRepository repo, AppDbContext context, IInventoryServiceClient inventoryService, ISalesServiceClient salesService)
        {
            _repo = repo;
            _context = context;
            _inventoryService = inventoryService;
            _salesService = salesService;
        }
        
        // all production orders for so
        public async Task<IEnumerable<ProductionOrderListDto>> GetAllProductionOrdersAsync(Guid? salesOrderId=null)
        {   
            var query = _context.ProductionOrders
                        .AsQueryable();

            // Filter Logic
            if (salesOrderId.HasValue)
            {
                query = query.Where(p => p.SalesOrderId == salesOrderId.Value);
            }

            var orders = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
            var result = new List<ProductionOrderListDto>();

            foreach(var p in orders)
            {
                var product = await _inventoryService.GetProductAsync(p.ProductId);
                result.Add(new ProductionOrderListDto
                {
                    ProductionOrderId = p.ProductionOrderId,
                    SalesOrderId = p.SalesOrderId,
                    ProductName = product?.Name?? "Unknown",
                    BatchQuantity = p.PlannedQuantity,
                    ProducedQuantity = p.ProducedQuantity,
                    ScrapQuantity = p.ScrapQuantity,
                    Status = p.Status,
                    PlannedDate = p.PlannedStartDate,
                    ActualStartDate = p.ActualStartDate,
                    ActualEndDate = p.ActualEndDate

                }); 
            }
            return result;
        }

        // DASHBOARD API: Pending Orders
        public async Task<IEnumerable<PendingOrderDto>> GetPendingSalesOrdersAsync()
        {
           var salesOrders = await _salesService.GetPendingSalesOrdersAsync();

            var list = new List<PendingOrderDto>();
            foreach (var so in salesOrders)
            {   
                // Product name HTTP se
                var product = await _inventoryService.GetProductAsync(so.ProductId);

                // fetch all batches for each so
                var allBatches = await _context.ProductionOrders
                                                .Where(p=> p.SalesOrderId == so.Id)
                                                .ToListAsync();
                // kitna produced hua
                decimal produced = allBatches
                                            .Where(p => p.Status == "Completed")
                                            .Sum(p => p.ProducedQuantity);
                // kitna pending
                decimal pipeline = allBatches
                                            .Where(p => p.Status != "Completed" && p.Status != "Cancelled")
                                            .Sum(p => p.PlannedQuantity);
                // kitna unplanned(bacha hai)
                decimal unplanned = so.Quantity - (produced + pipeline);
                if(unplanned < 0) unplanned = 0;

                int progress = 0;
                if (so.Quantity > 0)
                {
                    progress = (int)(produced / so.Quantity * 100);
                }
                // status logic
                string displayStatus = "New";
                if(pipeline > 0 && produced==0) displayStatus = "Planned";
                else if(produced > 0) displayStatus = "In Production";

                list.Add(new PendingOrderDto
                {
                    SalesOrderId = so.Id,
                    CustomerName = "", // TODO: Get customer name from sales API
                    ProductName = product?.Name??"Unknown",
                    OrderDate = DateTime.MinValue, // TODO: Get order date from sales API
                    TotalQuantity = so.Quantity,
                    ProducedQuantity = produced,
                    InPipelineQuantity = pipeline,
                    UnplannedQuantity = unplanned,
                    Status = displayStatus,
                    ProgressPercentage = progress
                });
            }
            return list;
        }

        //  INFO API: Planning Constraints
        public async Task<ProductionPlanningInfoDto> GetPlanningInfoAsync(Guid salesOrderId)
        {
            var so = await _salesService.GetSalesOrderAsync(salesOrderId);
            if (so == null) throw new Exception("Order not found");

            var product = await _inventoryService.GetProductAsync(so.ProductId);

            decimal planned = await _repo.GetTotalPlannedQtyBySalesOrderIdAsync(salesOrderId);
            
            // Material Check
            var bom = await _context.BOMs
                                     .Where(b => b.ProductId == so.ProductId && b.IsActive)
                                     .ToListAsync();

            var rmIds = bom.Select(b => b.RawMaterialId).ToList();

            var rawMaterials = await _inventoryService.GetRawMaterialsByIdsAsync(rmIds);

            decimal maxPossible = decimal.MaxValue;
            string limiter = "None";

            foreach(var item in bom)
            {
                if(item.QuantityRequired > 0)
                {
                    var rm = rawMaterials.FirstOrDefault(r => r.Id == item.RawMaterialId);
                    decimal avail = rm?.AvailableQuantity ?? 0;
                    decimal possible = Math.Floor(avail / item.QuantityRequired);
                    if(possible < maxPossible)
                    {
                        maxPossible = possible;
                        limiter = rm?.Name ?? "";
                    }
                }
            }

            // no material in BOM
            if (!bom.Any()) maxPossible = 0;

            // Smart Batch Suggestion — Max-Min K (Binary Search)
            decimal remaining = so.Quantity - planned;
            decimal capacity = product?.MaxDailyCapacity ?? 0;
            decimal available = Math.Min(remaining, maxPossible == decimal.MaxValue ? remaining : maxPossible);

            var suggestion = BatchOptimizer.GetOptimalBatchPlan(available, capacity);

            return new ProductionPlanningInfoDto
            {
                SalesOrderId = so.Id,
                ProductName = product?.Name ?? "",
                RemainingQuantity = remaining,
                MachineDailyCapacity = capacity,
                MaxPossibleByMaterial = maxPossible,
                LimitingMaterial = limiter,
                SuggestedBatches = suggestion.SuggestedBatches,
                SuggestedBatchSize = suggestion.SuggestedBatchSize,
                BatchSizes = suggestion.BatchSizes,
                MinEfficiency = suggestion.MinEfficiency,
                AvgEfficiency = suggestion.AvgEfficiency,
                IsOptimal = suggestion.IsOptimal
            };
        }

        // Create PO (Create Production Order)
        public async Task<string> CreateProductionPlanAsync(CreateProductionDto dto, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var so = await _salesService.GetSalesOrderAsync(dto.SalesOrderId);
                if (so == null) return "Sales Order not found";

                // Product info for capacity check
                var product = await _inventoryService.GetProductAsync(so.ProductId);

                decimal planned = await _repo.GetTotalPlannedQtyBySalesOrderIdAsync(dto.SalesOrderId);
                decimal totalRemaining = so.Quantity - planned;

                if (dto.QuantityToProduce > totalRemaining)
                    return "Error: Either Order is completed or you are creating more than remaining quantity";

                // Efficiency check — below 50% = machine waste
                if (!dto.ForceCreate)
                {
                    decimal maxCap = product?.MaxDailyCapacity ?? 0;
                    
                    if (maxCap > 0)
                    {
                        decimal efficiency = BatchOptimizer.GetEfficiency(dto.QuantityToProduce, maxCap);

                        if (efficiency < BatchOptimizer.MIN_EFFICIENCY_THRESHOLD)
                            return $"Efficiency Warning: Batch of {dto.QuantityToProduce} runs at only {efficiency}% machine efficiency. " +
                                   $"Minimum threshold: {BatchOptimizer.MIN_EFFICIENCY_THRESHOLD}%. Use ForceCreate=true to override.";
                    }
                }

                // Create PO (status = "Created", NO inventory reservation yet)
                var po = new ProdOrder
                {
                    SalesOrderId = so.Id,
                    ProductId = so.ProductId,
                    PlannedQuantity = dto.QuantityToProduce,
                    PlannedStartDate = dto.PlannedStartDate,
                    PlannedEndDate = dto.PlannedEndDate,
                    Status = "Created",
                    CreatedByUserId = userId
                };
                await _repo.AddAsync(po);

                // Update SO status via HTTP
                if (so.Status == "Pending")
                {
                    await _salesService.UpdateSalesOrderStatusAsync(so.Id, "In Production"); // TODO: Update SO status via HTTP
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return "Success";
            }
            catch(Exception ex) {
                await transaction.RollbackAsync();
                return $"Error: {ex.Message}";
            }
        }

        // RELEASE: Created → Released (Reserve Material via HTTP)
        public async Task<string> ReleaseProductionOrderAsync(Guid productionOrderId, Guid userId)
        {
            var po = await _context.ProductionOrders.FindAsync(productionOrderId);
            if (po == null) return "Order not found.";
            if (po.Status != "Created") return "Error: Only 'Created' orders can be Released.";

            // BOM se material list laao
            var boms = await _context.BOMs
                        .Where(b => b.ProductId == po.ProductId && b.IsActive)
                        .ToListAsync();

            if (!boms.Any()) return "Error: No BOM found for this product.";

            // Reserve via Inventory HTTP
            var items = boms.Select(b => new MaterialItem
            {
                RawMaterialId = b.RawMaterialId,
                Quantity = b.QuantityRequired * po.PlannedQuantity
            }).ToList();

            var reserved = await _inventoryService.ReserveMaterialsAsync(new InventoryReservationRequest
            {
                ProductionOrderId = po.ProductionOrderId,
                MovementType = "RESERVE",
                Items = items
            });

            if (!reserved) return "Error: Inventory service failed to reserve materials.";

            po.Status = "Released";
            po.UpdatedByUserId = userId;
            await _context.SaveChangesAsync();
            return "Success";
        }
        
        // UPDATE QTY: Only for "Created" status (before release)
        public async Task<string> UpdateProductionQtyAsync(Guid productionOrderId, decimal newQuantity, Guid userId)
        {
            if (newQuantity <= 0) return "Error: Quantity must be greater than 0.";

            var po = await _context.ProductionOrders.FindAsync(productionOrderId);
            if (po == null) return "Order not found.";
            if (po.Status != "Created") return "Error: Quantity can only be updated for 'Created' orders.";

            // Check remaining capacity
            var so = await _salesService.GetSalesOrderAsync(po.SalesOrderId);
            if (so == null) return "Error: Sales Order not found.";

            decimal otherPlanned = await _context.ProductionOrders
                .Where(p => p.SalesOrderId == po.SalesOrderId
                        && p.ProductionOrderId != po.ProductionOrderId
                        && p.Status != "Cancelled")
                .SumAsync(p => p.PlannedQuantity);

            decimal remaining = so.Quantity - otherPlanned;
            if (newQuantity > remaining)
                return $"Error: Max allowed is {remaining}. Other batches already planned.";

            po.PlannedQuantity = newQuantity;
            po.UpdatedByUserId = userId;
            await _context.SaveChangesAsync();
            return "Success";
        }
        //  START PRODUCTION
        public async Task<string> StartProductionAsync(Guid poId, Guid userId)
        {
            var po = await _context.ProductionOrders.FindAsync(poId);
            if(po == null || po.Status != "Release") return "Invalid Order or Status";

            po.Status = "In Progress";
            po.ActualStartDate = DateTime.Now;
            po.UpdatedByUserId = userId;

            // SO update via HTTP
            await _salesService.UpdateSalesOrderStatusAsync(po.SalesOrderId, "In Production");

            await _context.SaveChangesAsync();
            return "Success";
        }

        // COMPLETE PRODUCTION
        public async Task<string> CompleteProductionAsync(CompleteProductionDto dto, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var po = await _context.ProductionOrders.FindAsync(dto.ProductionOrderId);
                if (po == null) return "Order not found.";
                if (po.Status != "In Progress") return "Error: Order must be 'In Progress' to complete.";

                if (dto.ActualProducedQuantity > po.PlannedQuantity)
                    return $"Error: Cannot produce {dto.ActualProducedQuantity}. Max Plan was {po.PlannedQuantity}.";
                if (dto.ActualProducedQuantity < 0 || dto.ScrapQuantity < 0)
                    return "Error: Quantity cannot be negative.";

                decimal totalOutput = dto.ActualProducedQuantity + dto.ScrapQuantity;
                if (totalOutput > po.PlannedQuantity)
                    return $"Error: Total exceeds Planned Quantity {po.PlannedQuantity}.";
                if (dto.ScrapQuantity > dto.ActualProducedQuantity)
                    return $"Error: Scrap cannot exceed Produced Quantity.";

                decimal produced = dto.ActualProducedQuantity;
                decimal scrap = dto.ScrapQuantity;
                decimal unused = po.PlannedQuantity - totalOutput;

                // A. RETURN unused material via HTTP
                if (unused > 0)
                {
                    var boms = await _context.BOMs
                                .Where(b => b.ProductId == po.ProductId && b.IsActive)
                                .ToListAsync();

                    var items = boms.Select(b => new MaterialItem
                    {
                        RawMaterialId = b.RawMaterialId,
                        Quantity = b.QuantityRequired * unused
                    }).ToList();

                    await _inventoryService.ReturnMaterialsAsync(new InventoryReservationRequest
                    {
                        ProductionOrderId = po.ProductionOrderId,
                        MovementType = "UNUSED_RETURN",
                        Items = items
                    });
                }

                // B. ADD Finished Goods via HTTP
                if (produced > 0)
                {
                    await _inventoryService.AddFinishedGoodsAsync(new FinishedGoodsRequest
                    {
                        ProductionOrderId = po.ProductionOrderId,
                        ProductId = po.ProductId,
                        ProducedQuantity = produced,
                        ScrapQuantity = scrap,
                        MovementType = "PRODUCTION"
                    });
                }

                // C. UPDATE PO
                po.Status = "Completed";
                po.ActualEndDate = DateTime.Now;
                po.ProducedQuantity = produced;
                po.ScrapQuantity = scrap;
                po.UpdatedByUserId = userId;

                // D. Check SO completion
                decimal previousDone = await _context.ProductionOrders
                    .Where(p => p.SalesOrderId == po.SalesOrderId && p.Status == "Completed" && p.ProductionOrderId != po.ProductionOrderId)
                    .SumAsync(p => p.ProducedQuantity);

                decimal totalDone = previousDone + produced;
                var so = await _salesService.GetSalesOrderAsync(po.SalesOrderId);
                
                if (so != null && totalDone >= so.Quantity)
                    await _salesService.UpdateSalesOrderStatusAsync(po.SalesOrderId, "Completed");

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


        // cancel order
        public async Task<string> CancelProductionOrderAsync(Guid productionOrderId, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var po = await _context.ProductionOrders.FindAsync(productionOrderId);
                if (po == null) return "Order not found.";

                if (po.Status != "Created" && po.Status != "Released")
                    return "Error: Only 'Created' or 'Released' orders can be cancelled.";

                // INVENTORY RETURN via HTTP (only if Released — material reserved tha)
                if (po.Status == "Released")
                {
                    var boms = await _context.BOMs
                                .Where(b => b.ProductId == po.ProductId && b.IsActive)
                                .ToListAsync();

                    var items = boms.Select(b => new MaterialItem
                    {
                        RawMaterialId = b.RawMaterialId,
                        Quantity = b.QuantityRequired * po.PlannedQuantity
                    }).ToList();

                    await _inventoryService.ReturnMaterialsAsync(new InventoryReservationRequest
                    {
                        ProductionOrderId = po.ProductionOrderId,
                        MovementType = "CANCEL_RETURN",
                        Items = items
                    });
                }

                // UPDATE STATUS
                po.Status = "Cancelled";
                po.UpdatedByUserId = userId;
                po.ActualEndDate = DateTime.Now;

                // RESET SO if no active POs left
                bool hasActivePOs = await _context.ProductionOrders
                    .AnyAsync(p => p.SalesOrderId == po.SalesOrderId
                                && p.ProductionOrderId != po.ProductionOrderId
                                && p.Status != "Cancelled");

                if (!hasActivePOs)
                {
                    await _salesService.UpdateSalesOrderStatusAsync(po.SalesOrderId, "Pending");
                }

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
    }
}