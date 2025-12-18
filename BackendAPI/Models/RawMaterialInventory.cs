using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class RawMaterialInventory:AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int RawMaterialId { get; set; }
        
        [ForeignKey("RawMaterialId")]
        public virtual RawMaterial? RawMaterial { get; set; } 
        
        public decimal AvailableQuantity { get; set; } = 0; // Remaining Qty
    }
}