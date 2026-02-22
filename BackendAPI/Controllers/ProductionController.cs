using BackendAPI.Dtos.Production;
using BackendAPI.Services.Production;
using BackendAPI.Data;
using BackendAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductionController : ControllerBase
    {
        private readonly IProductionService _service;
        private readonly AppDbContext _context;

        public ProductionController(IProductionService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        // Get next auto-generated Order Number (frontend pre-fill ke liye)
        [HttpGet("next-order-number")]
        public async Task<IActionResult> GetNextOrderNumber()
        {
            var nextNumber = await OrderNumberGenerator.GenerateAsync(_context);
            return Ok(new { orderNumber = nextNumber });
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
                return Ok(new { message = "Production Order Created Successfully!" });
            
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
                return Ok(new { message = "Production Order Cancelled." });

            return BadRequest(new { error = result });
        }

        //  COMPLETE ENDPOINT (Updated to use DTO)
        [HttpPut("complete")]
        public async Task<IActionResult> CompleteProduction([FromBody] CompleteProductionDto dto)
        {
            Guid userId = Guid.NewGuid(); 
            var result = await _service.CompleteProductionAsync(dto, userId);

            if (result.StartsWith("Success"))
            {
                // Parse: "Success|Planned:300|Produced:280|Scrap:10|UnusedReturned:10"
                var parts = result.Split('|').Skip(1)
                    .Select(p => p.Split(':'))
                    .ToDictionary(p => p[0], p => decimal.Parse(p[1]));

                return Ok(new
                {
                    message = "Production Completed ✅",
                    summary = new
                    {
                        planned = parts.GetValueOrDefault("Planned"),
                        produced = parts.GetValueOrDefault("Produced"),
                        scrap = parts.GetValueOrDefault("Scrap"),
                        unusedReturned = parts.GetValueOrDefault("UnusedReturned")
                    }
                });
            }

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
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProductionOrder(Guid id, [FromBody] UpdateProductionDto dto)
        {
            Guid userId = Guid.NewGuid(); // TODO: JWT
            var result = await _service.UpdateProductionOrderAsync(id,dto.PlannedStartDate,dto.PlannedEndDate, dto.NewQuantity, userId);

            if (result == "Success")
                return Ok(new { message = "Order Updated." });

            return BadRequest(new { error = result });
        }

    }
}