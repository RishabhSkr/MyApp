using BackendAPI.HttpClients.Dtos;
using BackendAPI.HttpClients.Mock;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers;

/// <summary>
/// Mock Inventory API — Products + Raw Materials manage karo
/// Jab real Inventory Service ready hogi, yeh controller hata dena
/// </summary>
[Route("api/mock/inventory")]
[ApiController]
public class MockInventoryController : ControllerBase
{
    // ─── PRODUCTS ───

    [HttpGet("products")]
    public IActionResult GetAllProducts()
    {
        return Ok(MockDataStore.Products);
    }

    [HttpGet("products/{id}")]
    public IActionResult GetProduct(Guid id)
    {
        var product = MockDataStore.Products.FirstOrDefault(p => p.Id == id);
        if (product == null) return NotFound(new { error = "Product not found" });
        return Ok(product);
    }

    [HttpPost("products")]
    public IActionResult CreateProduct([FromBody] ProductDto dto)
    {
        if (dto.Id == Guid.Empty) dto.Id = Guid.NewGuid();
        MockDataStore.Products.Add(dto);
        return Ok(new { message = "Mock Product Created ✅", data = dto });
    }

    [HttpPut("products/{id}")]
    public IActionResult UpdateProduct(Guid id, [FromBody] ProductDto dto)
    {
        var product = MockDataStore.Products.FirstOrDefault(p => p.Id == id);
        if (product == null) return NotFound(new { error = "Product not found" });
        product.Name = dto.Name;
        product.MaxDailyCapacity = dto.MaxDailyCapacity;
        return Ok(new { message = "Updated ✅", data = product });
    }

    // ─── RAW MATERIALS ───

    [HttpGet("raw-materials")]
    public IActionResult GetAllRawMaterials()
    {
        return Ok(MockDataStore.RawMaterials);
    }

    [HttpGet("raw-materials/{id}")]
    public IActionResult GetRawMaterial(Guid id)
    {
        var rm = MockDataStore.RawMaterials.FirstOrDefault(r => r.Id == id);
        if (rm == null) return NotFound(new { error = "Raw Material not found" });
        return Ok(rm);
    }

    [HttpPost("raw-materials")]
    public IActionResult CreateRawMaterial([FromBody] RawMaterialDto dto)
    {
        if (dto.Id == Guid.Empty) dto.Id = Guid.NewGuid();
        MockDataStore.RawMaterials.Add(dto);
        return Ok(new { message = "Mock Raw Material Created ✅", data = dto });
    }

    [HttpPut("raw-materials/{id}")]
    public IActionResult UpdateRawMaterial(Guid id, [FromBody] RawMaterialDto dto)
    {
        var rm = MockDataStore.RawMaterials.FirstOrDefault(r => r.Id == id);
        if (rm == null) return NotFound(new { error = "Raw Material not found" });
        rm.Name = dto.Name;
        rm.SKU = dto.SKU;
        rm.UOM = dto.UOM;
        rm.AvailableQuantity = dto.AvailableQuantity;
        return Ok(new { message = "Updated ✅", data = rm });
    }
}
