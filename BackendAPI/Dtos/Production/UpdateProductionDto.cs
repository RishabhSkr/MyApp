using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Dtos.Production
{
    public class UpdateProductionDto
    {   
        public Guid ProductionOrderId { get; set; }
        public decimal NewQuantity { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
    }
}