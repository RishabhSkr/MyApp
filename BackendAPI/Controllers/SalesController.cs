using BackendAPI.Services.Sales;
using Microsoft.AspNetCore.Mvc;
using BackendAPI.Dtos.Product;
using BackendAPI.Exceptions;

namespace BackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISalesService _service;

    public SalesController(ISalesService service)
    {
        _service = service;
    }

    [HttpGet("pending-sales")]
    public async Task<IActionResult> GetPendingSalesOrders()
    {
        return Ok(await _service.GetPendingSalesOrdersAsync());
    }

    // [HttpGet("{id}")]
    // public async Task<IActionResult> GetById(int id)
    // {
    //     var product = await _service.GetByIdAsync(id);
    //     return Ok(product);
    // }
}

