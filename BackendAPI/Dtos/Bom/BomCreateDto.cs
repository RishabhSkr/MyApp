using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Dtos.Bom
{
    // Wrapper
    public class BomCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one material is required")]
        public List<BomCreateItemDto> BomItems { get; set; } = new();
        // Testing ke liye add karein (Baad mein hata dena)
        // public int CreatedByUserId { get; set; }
    }

    // Child DTO 
    public class BomCreateItemDto
    {
        [Required]
        public int RawMaterialId { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal QuantityRequired { get; set; }
    }
}