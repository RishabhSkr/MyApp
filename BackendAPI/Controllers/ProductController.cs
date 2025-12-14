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
        if(!ModelState.IsValid) 
            return BadRequest(ModelState);

        var product = await _service.CreateAsync(dto);
        return Ok(product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ProductUpdateDto dto)
    {
        if (!ModelState.IsValid)
        return BadRequest(ModelState);
        await _service.UpdateAsync(id,dto);
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
