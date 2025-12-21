using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Dtos.RawMaterial
{
    public class RawMaterialCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string SKU { get; set; } = string.Empty; // e.g., "RM-IRON-001"
    }
}