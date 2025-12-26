namespace BackendAPI.Dtos.Production
{
    // List of Production Orders for SO 
    public class ProductionOrderListDto
    {
        public int ProductionOrderId { get; set; }
        public int SalesOrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        public decimal BatchQuantity { get; set; }
        public string Status { get; set; } = string.Empty; // Planned, In Progress, Completed

        // Dates
        public DateTime? PlannedDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
    }
}