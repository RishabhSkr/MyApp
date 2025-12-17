namespace BackendAPI.Dtos.Bom
{
    // Parent DTO
    public class BomResponseDto
    {
        public int productId { get; set; }
        public string productName { get; set; } = string.Empty; 
        public List<BomResponseItemDto> BomItems { get; set; } = new();
    }

    // Child DTO (Output)
    public class BomResponseItemDto
    {
        public int RawMaterialId { get; set; }
        public string RawMaterialName { get; set; } = string.Empty; 
        public string SKU { get; set; } = string.Empty;
        public decimal QuantityRequired { get; set; }
    }
}