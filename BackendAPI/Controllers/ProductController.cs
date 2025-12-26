using BackendAPI.Services.Product;
using Microsoft.AspNetCore.Mvc;
using BackendAPI.Dtos.Product;
using BackendAPI.Exceptions;

namespace BackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        // TODO: Jab JWT lag jayega, tab User.Claims se nikalenge
        //? Hardcoded
        int currentUserId = 1; 
        var result = await _service.CreateAsync(dto, currentUserId);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ProductUpdateDto dto)
    {
        // TODO: Jab JWT lag jayega, tab User.Claims se nikalenge
        int currentUserId = 1; // Hardcoded for testing
        await _service.UpdateAsync(id, dto, currentUserId);
        return Ok("Product updated successfully");
    }   

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _service.GetByIdAsync(id);
        if (product == null) throw new NotFoundException("Product not found");
        await _service.DeleteAsync(id);
        return Ok("Product deleted successfully");
    }
}

