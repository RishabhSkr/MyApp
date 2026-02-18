using BackendAPI.Dtos.Production;
using BackendAPI.Services.Production;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionController : ControllerBase
    {
        private readonly IProductionService _service;

        public ProductionController(IProductionService service)
        {
            _service = service;
        }

        // 0.GET: api/Production/orders?salesOrderId=101
        [HttpGet("production-orders")]
        public async Task<IActionResult> GetAllOrders([FromQuery] Guid? salesOrderId)
        {
            var result = await _service.GetAllProductionOrdersAsync(salesOrderId);
            return Ok(result);
        }
        // 1. DASHBOARD: Pending Orders
        [HttpGet("dashboard/pending-orders")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _service.GetPendingSalesOrdersAsync();
            return Ok(result);
        }

        // 2. INFO: Planning Details (Constraints)
        [HttpGet("planning-info/{salesOrderId}")]
        public async Task<IActionResult> GetInfo(Guid salesOrderId)
        {
            try
            {
                var result = await _service.GetPlanningInfoAsync(salesOrderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // 3. CREATE: Make a Batch
        [HttpPost("create-plan")]
        public async Task<IActionResult> Plan([FromBody] CreateProductionDto dto) 
        {
            Guid userId = Guid.NewGuid(); // TODO: Replace with JWT User ID
            var result = await _service.CreateProductionPlanAsync(dto, userId);

            if (result == "Success")
                return Ok(new { message = "Production Planned Successfully!" });
            
            return BadRequest(new { error = result });
        }

        // 4. START: Worker Action
        [HttpPut("start/{id}")]
        public async Task<IActionResult> StartProduction(Guid id)
        {
            Guid userId = Guid.NewGuid(); 
            var result = await _service.StartProductionAsync(id, userId);

            if (result == "Success")
                return Ok(new { message = "Production Started. Timer Running!" });

            return BadRequest(new { error = result });
        }


         //  CANCEL ENDPOINT
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            Guid userId = Guid.NewGuid(); // TODO: JWT
            var result = await _service.CancelProductionOrderAsync(id, userId);

            if (result == "Success")
                return Ok(new { message = "Order Cancelled. Material Restored to Inventory. ✅" });

            return BadRequest(new { error = result });
        }

        //  COMPLETE ENDPOINT (Updated to use DTO)
        [HttpPut("complete")]
        public async Task<IActionResult> CompleteProduction([FromBody] CompleteProductionDto dto)
        {
            Guid userId = Guid.NewGuid(); 
            var result = await _service.CompleteProductionAsync(dto, userId);

            if (result == "Success")
                return Ok(new { message = "Production Completed. Stock Updated!" });

            return BadRequest(new { error = result });
        }

        // 5. RELEASE: Created → Released (Reserve Material)
        [HttpPut("release/{id}")]
        public async Task<IActionResult> ReleaseOrder(Guid id)
        {
            Guid userId = Guid.NewGuid(); // TODO: JWT
            var result = await _service.ReleaseProductionOrderAsync(id, userId);

            if (result == "Success")
                return Ok(new { message = "Order Released. Material Reserved! ✅" });

            return BadRequest(new { error = result });
        }

        // 6. UPDATE QTY: Only for "Created" orders
        [HttpPut("update-qty/{id}")]
        public async Task<IActionResult> UpdateQty(Guid id, [FromQuery] decimal newQuantity)
        {
            Guid userId = Guid.NewGuid(); // TODO: JWT
            var result = await _service.UpdateProductionQtyAsync(id, newQuantity, userId);

            if (result == "Success")
                return Ok(new { message = "Quantity Updated! ✅" });

            return BadRequest(new { error = result });
        }

    }
}