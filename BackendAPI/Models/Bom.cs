

namespace BackendAPI.Models;

public class Bom:AuditableEntity
{
    public Guid Id { get; set; }

    // Human-readable BOM Number (BOM-STEELWIDGETA-0001)
    public string BomNumber { get; set; } = string.Empty;
    // Version: 1.0 → 1.1 → 1.2 (auto-increment on update)
    public decimal Version { get; set; } = 1.0m;

    public Guid ProductId { get; set; }
    public Guid RawMaterialId { get; set; }
    public decimal QuantityRequired { get; set; }
}
