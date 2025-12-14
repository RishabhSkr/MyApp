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

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var result = await _service.GetBomByProductAsync(productId);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BomCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateBomAsync(dto);

            if (result == "Success")
                return Ok(new { message = "BOM Created Successfully" });

            return BadRequest(new { message = result });
        }   
    }
}