using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Dtos.Production
{
    public class CreateProductionDto
    {
        [Required]
        public int SalesOrderId { get; set; }

        // Quantity to Produce
        [Required]
        [Range(1, 1000000, ErrorMessage = "Quantity must be greater than 0")]
        public decimal QuantityToProduce { get; set; }

        //  Schedule (Planned)
        [Required]
        public DateTime PlannedStartDate { get; set; }

        // Planned (Target)
        [Required]
        public DateTime PlannedEndDate { get; set; }
    }
}