using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Dtos.Production
{
    public class CreateProductionDto
    {
        [Required]
        public Guid SalesOrderId { get; set; }

        [Required]
        [Range(1, 1000000, ErrorMessage = "Quantity must be greater than 0")]
        public decimal QuantityToProduce { get; set; }

        //  Schedule (Planned)
        [Required]
        public DateTime PlannedStartDate { get; set; }

        // Planned (Target)
        [Required]
        public DateTime PlannedEndDate { get; set; }

        // Optional: User apna custom OrderNumber de sakta hai
        // Agar empty hai toh system auto-generate karega (PO-YYYYMMDD-XXXX)
        
        public string? CustomOrderNumber { get; set; }

        // ForceCreate = true: suggested batch ignore karke
        public bool ForceCreate { get; set; } = false;
    }
}
