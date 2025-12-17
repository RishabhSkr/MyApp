using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class SalesOrder
    {
        [Key]
        public int SalesOrderId { get; set; }

        public string CustomerName { get; set; } = string.Empty; // e.g. "Rahul Traders"

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // "Pending Production", "In Production", "Ready to Dispatch"
        public string Status { get; set; } = "Pending";

        // Product (Kya becha?) 
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        // --- Quantity (Planned) ---
        public int Quantity { get; set; } 
        
        // link to Users
        public int CreatedByUserId { get; set; }
       [ForeignKey(nameof(CreatedByUserId))]
        public virtual User? CreatedByUser { get; set; }

    }
}