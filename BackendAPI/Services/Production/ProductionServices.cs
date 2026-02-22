using BackendAPI.Data;
using BackendAPI.Dtos.Production;
using BackendAPI.Repositories.ProductionRepository;
using Microsoft.EntityFrameworkCore;
using ProdOrder = BackendAPI.Models.ProductionOrder;
using BackendAPI.HttpClients;
using BackendAPI.HttpClients.Dtos;
using BackendAPI.Utilities;
using BackendAPI.Constants;

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
            Console.WriteLine(orders);
            foreach(var p in orders)
            {
                var product = await _inventoryService.GetProductAsync(p.ProductId);
                result.Add(new ProductionOrderListDto
                {
                    ProductionOrderId = p.ProductionOrderId,
                    OrderNumber = p.OrderNumber,
                    SalesOrderId = p.SalesOrderId,
                    ProductName = product?.Name?? EventStatus.UNKNOWN,
                    BatchQuantity = p.PlannedQuantity,
                    ProducedQuantity = p.ProducedQuantity,
                    ScrapQuantity = p.ScrapQuantity,
                    UnusedReturnedQuantity = p.UnusedReturnedQuantity,
                    Status = p.Status,
                    PlannedStartDate = p.PlannedStartDate,
                    PlannedEndDate = p.PlannedEndDate,
                    ActualStartDate = p.ActualStartDate,
                    ActualEndDate = p.ActualEndDate,
                    CreatedByUserId = p.CreatedByUserId,
                    UpdatedByUserId = p.UpdatedByUserId

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
                                            .Where(p => p.Status == EventStatus.COMPLETED)
                                            .Sum(p => p.ProducedQuantity);
                // kitna pending
                decimal pipeline = allBatches
                                            .Where(p => p.Status != EventStatus.COMPLETED && p.Status != EventStatus.CANCELLED)
                                            .Sum(p => p.PlannedQuantity);
                // Skip fully completed orders (produced enough + no active batches)
                if (produced >= so.Quantity && pipeline == 0)
                    continue;

                // kitna unplanned(bacha hai) — consistent with GetTotalPlannedQtyBySalesOrderIdAsync
                // produced(Completed→ProducedQty) + pipeline(Active→PlannedQty)
                decimal unplanned = so.Quantity - (produced + pipeline);

                if(unplanned < 0) unplanned = 0;

                int progress = 0;
                if (so.Quantity > 0)
                {
                    progress = (int)(produced / so.Quantity * 100);
                }
                // status logic
                string displayStatus = EventStatus.NEW;
                if(pipeline > 0 && produced==0) displayStatus = EventStatus.PLANNED;
                else if(produced > 0) displayStatus = EventStatus.IN_PROGRESS;

                list.Add(new PendingOrderDto
                {
                    SalesOrderId = so.Id,
                    CustomerName = "", // TODO: Get customer name from sales API
                    ProductName = product?.Name??EventStatus.UNKNOWN,
                    OrderDate = so.OrderDate, // TODO: Get order date from sales API
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
            string limiter = EventStatus.NONE;

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
                FullCapacityBatches = suggestion.FullCapacityBatches
            };
        }

        // Create PO ( Production Order)
        public async Task<string> CreateProductionPlanAsync(CreateProductionDto dto, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {   var existingBatches = await _context.ProductionOrders
                .Where(po => po.SalesOrderId == dto.SalesOrderId && po.Status != EventStatus.CANCELLED)
                .ToListAsync();
                
                var so = await _salesService.GetSalesOrderAsync(dto.SalesOrderId);
                if (so == null) return "Sales Order not found";

                // Product info for capacity check
                var product = await _inventoryService.GetProductAsync(so.ProductId);
                decimal maxCap = product?.MaxDailyCapacity ?? 0;

                // HARD LIMIT: Batch cannot exceed machine capacity 
                if (maxCap > 0 && dto.QuantityToProduce > maxCap)
                    return $"Error: Batch quantity ({dto.QuantityToProduce}) exceeds machine daily capacity ({maxCap}). " +
                           $"Maximum allowed per batch: {maxCap}. Use Smart Batch Suggestion for optimal batch sizes.";

                decimal planned = await _repo.GetTotalPlannedQtyBySalesOrderIdAsync(dto.SalesOrderId);
                decimal totalRemaining = so.Quantity - planned;

                if (dto.QuantityToProduce > totalRemaining)
                    return "Error: Either Order is completed or you are creating more than remaining quantity";

                // BUFFER CAP: Total planned cannot exceed 120% of SO quantity
                decimal totalAllPlanned = existingBatches.Sum(b => b.PlannedQuantity) + dto.QuantityToProduce;
                decimal maxAllowed = so.Quantity * 1.20m;  // 20% buffer
                if (!dto.ForceCreate && totalAllPlanned > maxAllowed)
                    return $"Error: Total planned ({totalAllPlanned}) exceeds 120% buffer of SO quantity ({so.Quantity}). " +
                           $"Max allowed: {maxAllowed}. Review your scrap rates.";
                // Efficiency check — below 50% = machine waste (ForceCreate se override )
                if (!dto.ForceCreate && maxCap > 0)
                {
                    decimal efficiency = BatchOptimizer.GetEfficiency(dto.QuantityToProduce, maxCap);

                    if (efficiency < BatchOptimizer.MIN_EFFICIENCY_THRESHOLD)
                        return $"Efficiency Warning: Batch of {dto.QuantityToProduce} runs at only {efficiency}% machine efficiency. " +
                               $"Minimum threshold: {BatchOptimizer.MIN_EFFICIENCY_THRESHOLD}%.";
                }

                // OrderNumber: Custom ya Auto-Generate
                string orderNumber;
                if (!string.IsNullOrWhiteSpace(dto.CustomOrderNumber))
                {
                    // Custom OrderNumber by User  — duplicate check
                    bool exists = await _context.ProductionOrders
                        .AnyAsync(p => p.OrderNumber == dto.CustomOrderNumber);
                    if (exists)
                        return $"Error: OrderNumber '{dto.CustomOrderNumber}' already exists. Use a different one.";
                    orderNumber = dto.CustomOrderNumber;
                }
                else
                {
                    // System auto-generate (PO-YYYYMMDD-XXXX)
                    orderNumber = await OrderNumberGenerator.GenerateAsync(_context);
                }

                // Create PO (status = "Created", NO inventory reservation yet)
                var po = new ProdOrder
                {
                    OrderNumber = orderNumber,
                    SalesOrderId = so.Id,
                    ProductId = so.ProductId,
                    PlannedQuantity = dto.QuantityToProduce,
                    PlannedStartDate = dto.PlannedStartDate,
                    PlannedEndDate = dto.PlannedEndDate,
                    Status = EventStatus.CREATED,
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
                return EventStatus.SUCCESS;
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
            if (po.Status != EventStatus.CREATED) return "Error: Only 'Created' orders can be Released.";

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
                MovementType = InventoryMovementType.RESERVE,
                Items = items
            });

            if (!reserved) return "Error: Inventory service failed to reserve materials.";

            po.Status = EventStatus.RELEASED;
            po.UpdatedByUserId = userId;
            await _context.SaveChangesAsync();
            return EventStatus.SUCCESS;
        }
        
        // UPDATE QTY: Only for "Created" status (before release)
        public async Task<string> UpdateProductionOrderAsync(Guid productionOrderId,DateTime plannedStartDate,DateTime plannedEndDate, decimal newQuantity, Guid userId)
        {
            if (newQuantity <= 0) return "Error: Quantity must be greater than 0.";

            var po = await _context.ProductionOrders.FindAsync(productionOrderId);
            if (po == null) return "Order not found.";
            if (po.Status != EventStatus.CREATED) return "Error: Quantity can only be updated for 'Created' orders.";

            // Check remaining capacity
            var so = await _salesService.GetSalesOrderAsync(po.SalesOrderId);
            if (so == null) return "Error: Sales Order not found.";

            decimal otherPlanned = await _context.ProductionOrders
                .Where(p => p.SalesOrderId == po.SalesOrderId
                        && p.ProductionOrderId != po.ProductionOrderId
                        && p.Status != EventStatus.CANCELLED)
                .SumAsync(p => p.PlannedQuantity);

            decimal remaining = so.Quantity - otherPlanned;
            if (newQuantity > remaining)
                return $"Error: Max allowed is {remaining}. Other batches already planned.";

            po.PlannedQuantity = newQuantity;
            po.PlannedStartDate = plannedStartDate;
            po.PlannedEndDate = plannedEndDate;
            po.UpdatedByUserId = userId;
            await _context.SaveChangesAsync();
            return EventStatus.SUCCESS;
        }
        //  START PRODUCTION
        public async Task<string> StartProductionAsync(Guid poId, Guid userId)
        {
            var po = await _context.ProductionOrders.FindAsync(poId);
            if(po == null || po.Status != EventStatus.RELEASED) return "Invalid Order or Status";

            po.Status = EventStatus.IN_PROGRESS;
            po.ActualStartDate = DateTime.Now;
            po.UpdatedByUserId = userId;

            // SO update via HTTP
            await _salesService.UpdateSalesOrderStatusAsync(po.SalesOrderId, EventStatus.IN_PROGRESS);

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
                if (po.Status != EventStatus.IN_PROGRESS) return "Error: Order must be 'In Progress' to complete.";

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
                        MovementType = InventoryMovementType.UNUSED_RETURN,
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
                        MovementType = InventoryMovementType.PRODUCTION
                    });
                }

                // C. UPDATE PO
                po.Status = EventStatus.COMPLETED;
                po.ActualEndDate = DateTime.Now;
                po.ProducedQuantity = produced;
                po.ScrapQuantity = scrap;
                po.UnusedReturnedQuantity = unused;
                po.UpdatedByUserId = userId;

                // D. Check SO completion
                decimal previousDone = await _context.ProductionOrders
                    .Where(p => p.SalesOrderId == po.SalesOrderId && p.Status == EventStatus.COMPLETED && p.ProductionOrderId != po.ProductionOrderId)
                    .SumAsync(p => p.ProducedQuantity);

                decimal totalDone = previousDone + produced;
                var so = await _salesService.GetSalesOrderAsync(po.SalesOrderId);
                
                if (so != null && totalDone >= so.Quantity)
                    await _salesService.UpdateSalesOrderStatusAsync(po.SalesOrderId, EventStatus.COMPLETED);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Detailed summary 
                return $"Success|Planned:{po.PlannedQuantity}|Produced:{produced}|Scrap:{scrap}|UnusedReturned:{unused}";
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

                if (po.Status != EventStatus.CREATED && po.Status != EventStatus.RELEASED)
                    return "Error: Only 'Created' or 'Released' orders can be cancelled.";

                // INVENTORY RETURN via HTTP (only if Released — material reserved tha)
                if (po.Status == EventStatus.RELEASED)
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
                        MovementType = InventoryMovementType.CANCEL_RETURN,
                        Items = items
                    });
                }

                // UPDATE STATUS
                po.Status = EventStatus.CANCELLED;
                po.UpdatedByUserId = userId;
                po.ActualEndDate = DateTime.Now;

                // RESET SO if no active POs left
                bool hasActivePOs = await _context.ProductionOrders
                    .AnyAsync(p => p.SalesOrderId == po.SalesOrderId
                                && p.ProductionOrderId != po.ProductionOrderId
                                && p.Status != EventStatus.CANCELLED);

                if (!hasActivePOs)
                {
                    await _salesService.UpdateSalesOrderStatusAsync(po.SalesOrderId, EventStatus.PENDING);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return EventStatus.SUCCESS;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return $"Error: {ex.Message}";
            }
        }
    }
}