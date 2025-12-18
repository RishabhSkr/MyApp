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

        [HttpPost("plan")]
        public async Task<IActionResult> CreatePlan([FromBody] CreateProductionDto dto)
        {
            int userId = 1; // TODO: JWT User

            var result = await _service.CreateProductionPlanAsync(dto, userId);

            if (result == "Success")
                return Ok(new { message = "Production Order Planned Successfully!" });

            // Agar Stock kam hai ya koi error hai, to 400 Bad Request
            return BadRequest(new { error = result });
        }
    }
}