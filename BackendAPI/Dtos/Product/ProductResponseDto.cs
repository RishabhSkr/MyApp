public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // --- New Audit Fields ---
    public int CreatedByUserId { get; set; }
    // public string? CreatedByUserName { get; set; } //TODO later (Advance Mapping)
    public DateTime CreatedAt { get; set; }
    
    // Update info (Optional)
    public int? UpdatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
}