namespace BackendAPI.Dtos.Production
{   // for PO order creation
    public class ProductionPlanningInfoDto
    {
        public Guid SalesOrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal RemainingQuantity { get; set; } 
        
        // Limits
        public decimal MachineDailyCapacity { get; set; }
        public decimal MaxPossibleByMaterial { get; set; }
        public string LimitingMaterial { get; set; } = string.Empty;

        // Smart Batch Suggestion
        public int SuggestedBatches { get; set; }
        public decimal SuggestedBatchSize { get; set; }
        public List<decimal> BatchSizes { get; set; } = new();   // Individual batch sizes
        public decimal MinEfficiency { get; set; }    // Worst batch efficiency %
        public decimal AvgEfficiency { get; set; }    // Average efficiency %
        public bool IsOptimal { get; set; }           // >= 95%
    }   
}