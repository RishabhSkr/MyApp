using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class FinishedGoodsInventory
    {
        [Key]
        public int Id { get; set; }

        
        public int ProductID { get; set; } 
        [ForeignKey(nameof(ProductID))]
        public virtual Product? Product { get; set; }
        
        public decimal AvailableQuantity { get; set; } = 0;

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}