using BackendAPI.Dtos.RawMaterial;
using BackendAPI.Services.RawMaterial;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RawMaterialController : ControllerBase
    {
        private readonly IRawMaterialService _service;

        public RawMaterialController(IRawMaterialService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        //  Create New Material
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RawMaterialCreateDto dto)
        {
            int userId = 1; // TODO: JWT
            var result = await _service.CreateRawMaterialAsync(dto, userId);

            if (result == "Success")
                return Ok(new { message = "Raw Material Created Successfully" });

            return BadRequest(new { error = result });
        }

        //  Add Stock (Inventory Update)
        [HttpPost("add-stock")]
        public async Task<IActionResult> AddStock([FromBody] StockUpdateDto dto)
        {
            int userId = 1; // TODO: JWT
            var result = await _service.AddStockAsync(dto, userId);

            if (result == "Success")
                return Ok(new { message = "Stock Added Successfully" });

            return BadRequest(new { error = result });
        }
    }
}