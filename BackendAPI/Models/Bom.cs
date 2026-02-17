

namespace BackendAPI.Models;

public class Bom:AuditableEntity
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid RawMaterialId { get; set; }
    public decimal QuantityRequired { get; set; }
}