namespace BackendAPI.HttpClients.Dtos;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public decimal MaxDailyCapacity { get; set; }
}