namespace BackendAPI.HttpClients.Dtos;

public class RawMaterialDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal AvailableQuantity { get; set; }
}