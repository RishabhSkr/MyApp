using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Dtos.Production
{
    public class CreateProductionDto
    {
        [Required]
        public Guid SalesOrderId { get; set; }

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

        // ForceCreate = true: User jaanta hai batch chhota hai, machine efficient nahi chalegi
        // but phir bhi create karna hai (suggested batch ignore karke)
        public bool ForceCreate { get; set; } = false;
    }
}