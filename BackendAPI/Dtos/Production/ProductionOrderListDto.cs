namespace BackendAPI.Dtos.Production
{
    // List of Production Orders for SO 
    public class ProductionOrderListDto
    {
        public Guid ProductionOrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public Guid SalesOrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public decimal BatchQuantity { get; set; }
        public decimal ProducedQuantity { get; set; } 
        public decimal ScrapQuantity { get; set; }
        public decimal UnusedReturnedQuantity { get; set; }
        public string Status { get; set; } = string.Empty; // Planned, In Progress, Completed

        // Dates
        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }

        public Guid CreatedByUserId { get; set; }
        public Guid UpdatedByUserId { get; set; }
    }
}