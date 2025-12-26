namespace BackendAPI.Dtos.Production
{   // for PO order creation
    public class ProductionPlanningInfoDto
    {
        public int SalesOrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal RemainingQuantity { get; set; } 
        
        // Limits
        public int MachineDailyCapacity { get; set; }
        public decimal MaxPossibleByMaterial { get; set; }
        public string LimitingMaterial { get; set; } = string.Empty;
    }
}