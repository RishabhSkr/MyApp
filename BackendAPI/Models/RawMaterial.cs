using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Models
{
    public class RawMaterial
    {
        [Key]
        public int RawMaterialID { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // e.g. "Wood Plank", "Milk"

        public string SKU { get; set; } = string.Empty;
        
        // Navigation: Ek RawMaterial ki entry Inventory me ho sakti hai
        // (for LINQ)
        public virtual RawMaterialInventory? Inventory { get; set; } 
    }
}