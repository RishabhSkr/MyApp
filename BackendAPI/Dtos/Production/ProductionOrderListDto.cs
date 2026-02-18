namespace BackendAPI.Dtos.Production
{
    // List of Production Orders for SO 
    public class ProductionOrderListDto
    {
        public Guid ProductionOrderId { get; set; }
        public Guid SalesOrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public decimal BatchQuantity { get; set; }
        public decimal ProducedQuantity { get; set; } 
        public decimal ScrapQuantity { get; set; }
        public string Status { get; set; } = string.Empty; // Planned, In Progress, Completed

        // Dates
        public DateTime? PlannedDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
    }
}