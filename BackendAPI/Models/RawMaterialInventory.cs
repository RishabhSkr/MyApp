using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class RawMaterialInventory
    {
        [Key]
        public int Id { get; set; }

        public int RawMaterialId { get; set; }
        
        [ForeignKey("RawMaterialId")]
        public virtual RawMaterial? RawMaterial { get; set; } 
        
        public decimal AvailableQuantity { get; set; } = 0; // Remaining Qty
        
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // updated by
        public int UpdatedByUserId { get; set; }
        
        [ForeignKey("UpdatedByUserId")]
        public virtual User? UpdatedByUserIdUser { get; set; }
    }
}