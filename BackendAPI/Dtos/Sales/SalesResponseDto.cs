namespace BackendAPI.Dtos.Sales;

public class SalesResponseDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Quantity { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? ActualProductionStartDate { get; set; }
    public DateTime? ActualProductionEndDate { get; set; }
}