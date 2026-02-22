using BackendAPI.HttpClients.Dtos;
using BackendAPI.HttpClients.Mock;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers;

/// <summary>
/// Mock Sales API — Postman se test data manage karo
/// Jab real Sales Service ready hogi, yeh controller hata dena
/// </summary>
[Route("api/mock/sales")]
[ApiController]
public class MockSalesController : ControllerBase
{
    // GET: All Sales Orders
    [HttpGet("orders")]
    public IActionResult GetAll()
    {
        return Ok(MockDataStore.SalesOrders);
    }

    // GET: Single Sales Order
    [HttpGet("orders/{id}")]
    public IActionResult GetById(Guid id)
    {
        var order = MockDataStore.SalesOrders.FirstOrDefault(o => o.Id == id);
        if (order == null) return NotFound(new { error = "Sales Order not found" });
        return Ok(order);
    }

    // POST: Add a Sales Order
    [HttpPost("orders")]
    public IActionResult Create([FromBody] SalesOrderDto dto)
    {
        if (dto.Id == Guid.Empty) dto.Id = Guid.NewGuid();
        MockDataStore.SalesOrders.Add(dto);
        return Ok(new { message = "Mock Sales Order Created ✅", data = dto });
    }

    // PUT: Update Status
    [HttpPut("orders/{id}/status")]
    public IActionResult UpdateStatus(Guid id, [FromQuery] string status)
    {
        var order = MockDataStore.SalesOrders.FirstOrDefault(o => o.Id == id);
        if (order == null) return NotFound(new { error = "Sales Order not found" });
        order.Status = status;
        return Ok(new { message = $"Status updated to '{status}' ✅", data = order });
    }

    // DELETE: Remove
    [HttpDelete("orders/{id}")]
    public IActionResult Delete(Guid id)
    {
        var order = MockDataStore.SalesOrders.FirstOrDefault(o => o.Id == id);
        if (order == null) return NotFound(new { error = "Sales Order not found" });
        MockDataStore.SalesOrders.Remove(order);
        return Ok(new { message = "Deleted ✅" });
    }
}
