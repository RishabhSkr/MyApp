using BackendAPI.Models;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;
using BackendAPI.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
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
        Console.WriteLine(id.ToString(), dto);
        if (!ModelState.IsValid)
        return BadRequest(ModelState);
        await _service.UpdateAsync(id,dto);
        return NoContent();
    }   

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
