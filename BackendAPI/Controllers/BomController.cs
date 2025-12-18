using BackendAPI.Dtos.Bom;
using BackendAPI.Services.Bom;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BomController : ControllerBase
    {
        private readonly IBomService _service;

        public BomController(IBomService service)
        {
            _service = service;
        }

        // 1. GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllBomsAsync();
            return Ok(result);
        }

        // 2. GET BY ID (Already Done - Just for Reference)
        [HttpGet("{productId}")]
        public async Task<IActionResult> Get(int productId)
        {
            var result = await _service.GetBomByProductAsync(productId);
            if (result == null) return NotFound("BOM not found.");
            return Ok(result);
        }

        // 3. CREATE (Already Done - Added UserId)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BomCreateDto dto)
        {
            int userId = 1; // TODO: JWT User
            var result = await _service.CreateBomAsync(dto, userId);

            if (result == "Success") return Ok(new { message = "BOM Created" });
            return BadRequest(new { error = result });
        }

        // 4. UPDATE (New)
        [HttpPut("{productId}")]
        public async Task<IActionResult> Update(int productId, [FromBody] BomCreateDto dto)
        {
            int userId = 1; // TODO: JWT User
            
            // Note: Update logic me hum purana hata kar naya daal rahe hain
            var result = await _service.UpdateBomAsync(productId, dto, userId);

            if (result == "Success") return Ok(new { message = "BOM Updated Successfully" });
            return BadRequest(new { error = result });
        }

        // 5. DELETE (New)
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var result = await _service.DeleteBomAsync(productId);

            if (result == "Success") return Ok(new { message = "BOM Deleted Successfully" });
            return BadRequest(new { error = result });
        }
    }
}