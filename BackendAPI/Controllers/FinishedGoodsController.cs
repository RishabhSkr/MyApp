using BackendAPI.Services.FinishedGoods;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinishedGoodsController : ControllerBase
    {
        private readonly IFinishedGoodsService _service;

        public FinishedGoodsController(IFinishedGoodsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetStock()
        {
            var stock = await _service.GetCurrentStockAsync();
            return Ok(stock);
        }
    }
}