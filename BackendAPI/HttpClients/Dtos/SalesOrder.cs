namespace BackendAPI.HttpClients.Dtos;

public class SalesOrderDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
}