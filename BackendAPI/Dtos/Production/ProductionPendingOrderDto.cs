namespace BackendAPI.Dtos.Production
{
    public class PendingOrderDto
    {
        public int SalesOrderId { get; set; }
        public string CustomerName { get; set; }=string.Empty;
        public string ProductName { get; set; }=string.Empty;
        public DateTime OrderDate { get; set; }

        // Raw Numbers
        public int TotalQuantity { get; set; }
        public decimal ProducedQuantity { get; set; }
        public decimal InPipelineQuantity { get; set; }
        public decimal UnplannedQuantity { get; set; }
        
        public string Status { get; set; }=string.Empty;
        public int ProgressPercentage { get; set; } 
    }
}

// namespace BackendAPI.Dtos.Production
// {
//     //  Dashboard for Pending Production Orders
//     public class PendingOrderDto 
//     {
//         public int SalesOrderId { get; set; }
//         public string CustomerName { get; set; } = string.Empty;
//         public string ProductName { get; set; } = string.Empty;
//         public DateTime OrderDate { get; set; }
        
//         // Status Info
//         public int TotalQuantity { get; set; }
//         public decimal PlannedQuantity { get; set; }
//         public decimal RemainingQuantity { get; set; }
//     }
// }