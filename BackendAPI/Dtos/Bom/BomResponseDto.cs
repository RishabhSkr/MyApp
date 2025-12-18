namespace BackendAPI.Dtos.Bom
{
    public class BomResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        
        // List of Materials
        public List<BomItemDto> Materials { get; set; } = new();
    }

    // 2. Child DTO 
    public class BomItemDto
    {
        public int RawMaterialId { get; set; }
        public string RawMaterialName { get; set; } = string.Empty;
         public string SKU { get; set; } = string.Empty;
        public decimal QuantityRequired { get; set; }
        public string Unit { get; set; } = "Unit";
    }
}