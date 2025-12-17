using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class FinishedGoodsInventory
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; } 
        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }
        
        public decimal AvailableQuantity { get; set; } = 0;
        // public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public int UpdatedByUserId { get; set; }
        [ForeignKey("UpdatedByUserId")]
        public virtual User? UpdatedByUserIdUser { get; set; }
    }
}