using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class SalesOrder:AuditableEntity
    {
        [Key]
        public int SalesOrderId { get; set; }

        public string CustomerName { get; set; } = string.Empty; 

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // "Pending", "In-Transit", "Completed"
        public string Status { get; set; } = "Pending";

        // Product (Kya becha?) 
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
       
        // Project Start & End Dates
        public DateTime? ActualProductionStartDate { get; set; } 
        public DateTime? ActualProductionEndDate { get; set; }
        
        // Quantity (Planned) 
        public int Quantity { get; set; } 

    }
}