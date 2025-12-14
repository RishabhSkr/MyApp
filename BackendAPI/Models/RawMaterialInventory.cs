using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class RawMaterialInventory
    {
        [Key]
        public int InventoryID { get; set; }

        public int RawMaterialID { get; set; }
        
        [ForeignKey("RawMaterialID")]
        public virtual RawMaterial? RawMaterial { get; set; } 
        
        public decimal AvailableQuantity { get; set; } = 0; // Remaining Qty
        
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}