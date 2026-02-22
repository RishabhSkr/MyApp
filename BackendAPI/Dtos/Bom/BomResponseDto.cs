using BackendAPI.Models;

namespace BackendAPI.Dtos.Bom
{
    public class BomResponseDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string BomNumber { get; set; } = string.Empty;
        public decimal Version { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid? UpdatedByUserId { get; set; }

        // List of Materials
        public List<BomItemDto> Materials { get; set; } = new();
    }

    // 2. Child DTO 
    public class BomItemDto
    {
        public Guid RawMaterialId { get; set; }
        public string RawMaterialName { get; set; } = string.Empty;
         public string SKU { get; set; } = string.Empty;
        public decimal QuantityRequired { get; set; }
        public string? UOM { get; set; }

    }
}