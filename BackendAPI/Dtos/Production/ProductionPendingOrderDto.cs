namespace BackendAPI.Dtos.Production
{
    public class PendingOrderDto
    {
        public Guid SalesOrderId { get; set; }
        public string CustomerName { get; set; }=string.Empty;
        public string ProductName { get; set; }=string.Empty;
        public DateTime OrderDate { get; set; }

        // Raw Numbers
        public decimal TotalQuantity { get; set; }
        public decimal ProducedQuantity { get; set; }
        public decimal InPipelineQuantity { get; set; }
        public decimal UnplannedQuantity { get; set; }
        
        public string Status { get; set; }=string.Empty;
        public int ProgressPercentage { get; set; } 
    }
}