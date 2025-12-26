using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Dtos.Production
{
    public class CompleteProductionDto
    {
        [Required]
        public int ProductionOrderId { get; set; }

        // Worker batayega ki machine kharab hone tak kitna bana
        [Required]
        [Range(0, 1000000, ErrorMessage = "Quantity must be positive.")]
        public decimal ActualProducedQuantity { get; set; } 

        [Required]
        [Range(0, 1000000, ErrorMessage = "Quantity must be zero or positive.")]
        public decimal ScrapQuantity { get; set; }
    }
}