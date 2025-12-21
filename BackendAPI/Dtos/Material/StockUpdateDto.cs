using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Dtos.RawMaterial
{
    public class StockUpdateDto
    {
        [Required]
        public int RawMaterialId { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }
    }
}