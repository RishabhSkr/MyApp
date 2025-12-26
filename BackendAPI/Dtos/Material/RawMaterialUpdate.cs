using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Dtos.RawMaterial
{
    public class RawMaterialUpdateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string SKU { get; set; } = string.Empty; 

        [Required]
        public string UOM { get; set; } = string.Empty;
    }
}