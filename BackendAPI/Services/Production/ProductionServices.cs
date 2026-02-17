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
        private readonly AppDbContext _context; 

        public ProductionService(IProductionRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }
        
        // all production orders for so
        public async Task<IEnumerable<ProductionOrderListDto>> GetAllProductionOrdersAsync(int? salesOrderId=null)
        {   
            var query = _context.ProductionOrders
                        .Include(p => p.Product)
                        .AsQueryable();

            // Filter Logic
            if (salesOrderId.HasValue)
            {
                query = query.Where(p => p.SalesOrderId == salesOrderId.Value);
            }

            var orders = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
            
            return orders.Select(p => new ProductionOrderListDto
            {
                ProductionOrderId = p.ProductionOrderId,
                SalesOrderId = p.SalesOrderId,
                ProductName = p.Product?.Name?? "Unknown",
                BatchQuantity = p.PlannedQuantity,
                ProducedQuantity = p.ProducedQuantity,
                ScrapQuantity = p.ScrapQuantity,
                Status = p.Status,
                PlannedDate = p.PlannedStartDate,
                ActualStartDate = p.ActualStartDate,
                ActualEndDate = p.ActualEndDate

            }).ToList(); 
        }

        // DASHBOARD API: Pending Orders
        public async Task<IEnumerable<PendingOrderDto>> GetPendingSalesOrdersAsync()
        {
            var salesOrders = await _context.SalesOrders
                                            .Include(s => s.Product)
                                            .Where(s => s.Status != "Completed")
                                            .OrderBy(s => s.OrderDate)
                                            .ToListAsync();

            var list = new List<PendingOrderDto>();
            foreach (var so in salesOrders)
            {   

                // fetch all batches for each so
                var allBatches = await _context.ProductionOrders
                                                .Where(p=> p.SalesOrderId == so.SalesOrderId)
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
                    SalesOrderId = so.SalesOrderId,
                    CustomerName = so.CustomerName,
                    ProductName = so.Product?.Name??"Unknown",
                    OrderDate = so.OrderDate,
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
        public async Task<ProductionPlanningInfoDto> GetPlanningInfoAsync(int salesOrderId)
        {
            var so = await _context.SalesOrders.Include(s => s.Product).FirstOrDefaultAsync(s => s.SalesOrderId == salesOrderId);
            if (so == null) throw new Exception("Order not found");

            decimal planned = await _repo.GetTotalPlannedQtyBySalesOrderIdAsync(salesOrderId);
            
            // Material Check
            var bom = await _context.BOMs
                                     .Include(b => b.RawMaterial).ThenInclude(rm => rm!.Inventory)
                                     .Where(b => b.ProductId == so.ProductId && b.IsActive)
                                     .ToListAsync();

            decimal maxPossible = decimal.MaxValue;
            string limiter = "None";

            foreach(var item in bom)
            {
                if(item.QuantityRequired > 0)
                {
                    decimal avail = item.RawMaterial?.Inventory?.AvailableQuantity ?? 0;
                    decimal possible = Math.Floor(avail / item.QuantityRequired);
                    if(possible < maxPossible)
                    {
                        maxPossible = possible;
                        limiter = item.RawMaterial?.Name ?? "";
                    }
                }
            }
            // no material in BOM
            if (!bom.Any()) maxPossible = 0;

            return new ProductionPlanningInfoDto
            {
                SalesOrderId = so.SalesOrderId,
                ProductName = so.Product?.Name ?? "",
                RemainingQuantity = so.Quantity - planned,
                MachineDailyCapacity = so.Product?.MaxDailyCapacity ?? 0,
                MaxPossibleByMaterial = maxPossible,
                LimitingMaterial = limiter
            };
        }

        // CREATE PLAN (Reserve Material)
        public async Task<string> CreateProductionPlanAsync(CreateProductionDto dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var so = await _context.SalesOrders.Include(s => s.Product).FirstOrDefaultAsync(s => s.SalesOrderId == dto.SalesOrderId);
                if (so == null) return "Sales Order not found";

                // Validations
                int maxCap = so.Product?.MaxDailyCapacity ?? 100;
                if (dto.QuantityToProduce > maxCap) return $"Capacity Alert: Max limit is {maxCap}";

                decimal planned = await _repo.GetTotalPlannedQtyBySalesOrderIdAsync(dto.SalesOrderId);
                if (dto.QuantityToProduce > (so.Quantity - planned)) return "Error: Either Order is completed or you are creating more than remaining quantity";

                // Inventory Deduction
                var boms = await _context.BOMs.Include(b => b.RawMaterial).ThenInclude(i => i!.Inventory).Where(b => b.ProductId == so.ProductId && b.IsActive).ToListAsync();
                
                // Pehle Check
                foreach(var b in boms) {
                    decimal req = b.QuantityRequired * dto.QuantityToProduce;
                    if((b.RawMaterial?.Inventory?.AvailableQuantity ?? 0) < req) return $"Not enough stock for {b.RawMaterial?.Name} in Inventory. Required: {req} Available: {b.RawMaterial?.UOM}: {b.RawMaterial?.Inventory?.AvailableQuantity}";
                }

                // Fir Deduct (Reserve)
                foreach(var b in boms) {
                    if(b.RawMaterial?.Inventory != null) {
                        b.RawMaterial.Inventory.AvailableQuantity -= (b.QuantityRequired * dto.QuantityToProduce);
                        _context.RawMaterialInventories.Update(b.RawMaterial.Inventory);
                    }
                }

                // Create PO
                var po = new ProdOrder {
                    SalesOrderId = so.SalesOrderId,
                    ProductId = so.ProductId,
                    PlannedQuantity = dto.QuantityToProduce,
                    PlannedStartDate = dto.PlannedStartDate,
                    PlannedEndDate = dto.PlannedEndDate,
                    Status = "Planned",
                    CreatedByUserId = userId
                };
                await _repo.AddAsync(po);

                // Update SO
                if(so.Status == "Pending") {
                    so.Status = "In Production";
                    _context.SalesOrders.Update(so);
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

        //  START PRODUCTION
        public async Task<string> StartProductionAsync(int poId, int userId)
        {
            var po = await _context.ProductionOrders.FindAsync(poId);
            if(po == null || po.Status != "Planned") return "Invalid Order or Status";

            po.Status = "In Progress";
            po.ActualStartDate = DateTime.Now;
            po.UpdatedByUserId = userId;

            var so = await _context.SalesOrders.FindAsync(po.SalesOrderId);
            if(so != null && so.ActualProductionStartDate == null) {
                so.ActualProductionStartDate = DateTime.Now; // Project Start
            }

            await _context.SaveChangesAsync();
            return "Success";
        }

        // COMPLETE PRODUCTION (Add Finished Goods)
        // public async Task<string> CompleteProductionAsync(int productionOrderId, int userId)
        // {
        //     using var transaction = await _context.Database.BeginTransactionAsync();
        //     try {
        //         var prodOrder = await _context.ProductionOrders.FindAsync(productionOrderId);
        //         if(prodOrder == null || prodOrder.Status != "In Progress") return "Invalid Order or Status";

        //         // Update PO
        //         prodOrder.Status = "Completed";
        //         prodOrder.ActualEndDate = DateTime.Now;
        //         prodOrder.ProducedQuantity = prodOrder.PlannedQuantity; // Standard Consumption Logic
        //         prodOrder.UpdatedByUserId = userId;

        //         // Add to Finished Goods Inventory
        //         var fgInv = await _context.FinishedGoodsInventories.FirstOrDefaultAsync(f => f.ProductId == prodOrder.ProductId);
        //         if(fgInv == null) {
        //             fgInv = new FinishedGoodsInventory { ProductId = prodOrder.ProductId, AvailableQuantity = 0, CreatedByUserId = userId };
        //             _context.FinishedGoodsInventories.Add(fgInv);
        //         }
        //         fgInv.AvailableQuantity += prodOrder.ProducedQuantity;

        //         // Check Project Completion
        //         var saleOrder = await _context.SalesOrders.FindAsync(prodOrder.SalesOrderId);
        //         if(saleOrder != null) {
        //             decimal totalDone = await _context.ProductionOrders.Where(p => p.SalesOrderId == prodOrder.SalesOrderId && p.Status == "Completed").SumAsync(p => p.ProducedQuantity);
        //             totalDone += prodOrder.ProducedQuantity; // Current wala

        //             if(totalDone >= saleOrder.Quantity) {
        //                 saleOrder.Status = "Completed";
        //                 saleOrder.ActualProductionEndDate = DateTime.Now;
        //             }
        //         }

        //         await _context.SaveChangesAsync();
        //         await transaction.CommitAsync();
        //         return "Success";
        //     }
        //     catch(Exception ex) {
        //         await transaction.RollbackAsync();
        //         return $"Error: {ex.Message}";
        //     }
        // }

        public async Task<string> CompleteProductionAsync(CompleteProductionDto dto, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var po = await _context.ProductionOrders.FindAsync(dto.ProductionOrderId);
                if (po == null) return "Order not found.";
                if (po.Status != "In Progress") return "Error: Order must be 'In Progress' to complete.";

                // Validation
                if (dto.ActualProducedQuantity > po.PlannedQuantity)
                    return $"Error: Cannot produce {dto.ActualProducedQuantity}. Max Plan was {po.PlannedQuantity}.";

                if (dto.ActualProducedQuantity < 0 || dto.ScrapQuantity < 0)
                {
                    return "Error: Quantity cannot be negative.";
                }

                decimal totalOutput = dto.ActualProducedQuantity + dto.ScrapQuantity;
                
                if (totalOutput > po.PlannedQuantity)
                {
                    return $"Error: Total (Produced {dto.ActualProducedQuantity} + Scrap {dto.ScrapQuantity}) cannot exceed Planned Quantity {po.PlannedQuantity}.";
                }
                if (dto.ScrapQuantity > dto.ActualProducedQuantity)
                {
                    return $"Error: Abnormal Scrap Level! Scrap ({dto.ScrapQuantity}) cannot be more than Produced Quantity ({dto.ActualProducedQuantity}). Please check inputs.";
                }
                // Calculation
                decimal produced = dto.ActualProducedQuantity;
                decimal scrap = dto.ScrapQuantity;
                decimal unused = po.PlannedQuantity - totalOutput; 
                decimal planned = po.PlannedQuantity;

                // A. PARTIAL INVENTORY RESTORE (Short Close Logic)
                if (unused > 0)
                {
                    var boms = await _context.BOMs
                                            .Include(b => b.RawMaterial).ThenInclude(rm => rm!.Inventory)
                                            .Where(b => b.ProductId == po.ProductId && b.IsActive)
                                            .ToListAsync();

                    foreach (var item in boms)
                    {
                        if (item.RawMaterial?.Inventory != null)
                        {
                            decimal restoreQty = item.QuantityRequired * unused;
                            item.RawMaterial.Inventory.AvailableQuantity += restoreQty; // Partial Add
                            _context.RawMaterialInventories.Update(item.RawMaterial.Inventory);
                        }
                    }
                }

                // B. ADD FINISHED GOODS (Real Qty) 
                if (produced > 0)
                {
                    var fgInv = await _context.FinishedGoodsInventories.FirstOrDefaultAsync(f => f.ProductId == po.ProductId);
                    if (fgInv == null)
                    {
                        fgInv = new FinishedGoodsInventory { ProductId = po.ProductId, AvailableQuantity = 0, CreatedByUserId = userId };
                        _context.FinishedGoodsInventories.Add(fgInv);
                    }
                    fgInv.AvailableQuantity += produced;
                    _context.FinishedGoodsInventories.Update(fgInv);
                }

                // C. UPDATE PO
                po.Status = "Completed";
                po.ActualEndDate = DateTime.Now;
                po.ProducedQuantity = produced;
                po.ScrapQuantity = scrap;
                po.UpdatedByUserId = userId;

                // D. SALES ORDER UPDATE
                var so = await _context.SalesOrders.FindAsync(po.SalesOrderId);
                if (so != null)
                {
                    // Ab Repo naya logic use karega (Completed wala Produced count karega)
                    // Humen current batch ka 'Produced' add karna padega total me check ke liye
                    // Note: Behtar ye hai ki hum Context save karne ke baad check karein, par transaction hai to manual count karte hain:
                    
                    decimal previousDone = await _context.ProductionOrders
                        .Where(p => p.SalesOrderId == po.SalesOrderId && p.Status == "Completed" && p.ProductionOrderId != po.ProductionOrderId)
                        .SumAsync(p => p.ProducedQuantity);
                    
                    decimal totalDone = previousDone + produced;

                    if (totalDone >= so.Quantity)
                    {
                        so.Status = "Completed";
                        so.ActualProductionEndDate = DateTime.Now;
                    }
                    else
                    {
                        so.Status = "In Production"; // Short close hua to bhi 'In Production' rahega
                    }
                    _context.SalesOrders.Update(so);
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
        // cancel order
        public async Task<string> CancelProductionOrderAsync(int productionOrderId, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var po = await _context.ProductionOrders.FindAsync(productionOrderId);
                if (po == null) return "Order not found.";
                
                // Validation
                if (po.Status != "Planned") 
                    return "Error: Only 'Planned' orders can be fully Cancelled. If started, use Complete (Short Close).";

                //  INVENTORY RESTORE 
                // add to inventory
                var boms = await _context.BOMs
                                        .Include(b => b.RawMaterial).ThenInclude(rm => rm!.Inventory)
                                        .Where(b => b.ProductId == po.ProductId && b.IsActive)
                                        .ToListAsync();

                foreach (var item in boms)
                {
                    if (item.RawMaterial?.Inventory != null)
                    {
                        decimal restoreQty = item.QuantityRequired * po.PlannedQuantity;
                        item.RawMaterial.Inventory.AvailableQuantity += restoreQty; // 📈 Full Add
                        item.RawMaterial.Inventory.UpdatedByUserId = userId;
                        _context.RawMaterialInventories.Update(item.RawMaterial.Inventory);
                    }
                }

                //  UPDATE STATUS
                po.Status = "Cancelled";
                po.UpdatedByUserId = userId;
                po.ActualEndDate = DateTime.Now; // Cancel time

                //  RESET SALES ORDER (Agar aur koi active PO nahi bacha)
                bool hasActivePOs = await _context.ProductionOrders
                    .AnyAsync(p => p.SalesOrderId == po.SalesOrderId 
                                && p.ProductionOrderId != po.ProductionOrderId 
                                && p.Status != "Cancelled");

                if (!hasActivePOs)
                {
                    var so = await _context.SalesOrders.FindAsync(po.SalesOrderId);
                    if (so != null && so.Status != "Completed")
                    {
                        so.Status = "Pending"; // Wapas shuruat par
                        so.ActualProductionStartDate = null;
                        _context.SalesOrders.Update(so);
                    }
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