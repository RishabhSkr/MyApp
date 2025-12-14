using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Dtos.Bom
{
    public class BomItemDto
    {
        [Required]
        public int RawMaterialId { get; set; }
        [Required]
        public string? RawMaterialName { get; set; }
        [Required]
        [Range(0.01, 100000)]
        public decimal QuantityRequired { get; set; }
    }
}