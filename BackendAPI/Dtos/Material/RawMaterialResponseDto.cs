public class RawMaterialResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string UOM { get; set; } = string.Empty;
    public List<RawMaterialInventoryResponseDto> Inventories { get; set; } = new();
    // --- New Audit Fields ---
    public int CreatedByUserId { get; set; }
    // public string? CreatedByUserName { get; set; } //TODO later (Advance Mapping)
    public DateTime CreatedAt { get; set; }
    
    // Update info (Optional)
    public int? UpdatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class RawMaterialInventoryResponseDto
{
    public int InventoryId { get; set; }
    public decimal AvailableQuantity { get; set; }
}