using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class FinishedGoodsInventory:AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; } 
        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }
        
        public decimal AvailableQuantity { get; set; } = 0;
    }
}