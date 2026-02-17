namespace BackendAPI.Models;
public class ProductionOrder : AuditableEntity
{
    public Guid ProductionOrderId { get; set; } = Guid.NewGuid();
    // Link to SalesOrder
    public Guid SalesOrderId { get; set; }
    // Link to Product
    public Guid ProductId { get; set; }
    //  Qty
    public decimal PlannedQuantity { get; set; }
    public decimal ProducedQuantity { get; set; } = 0;
    public decimal ScrapQuantity { get; set; } = 0;
    // State -> Planned - Release-Inprogress- Completed-Cancelled
    public string Status { get; set; }="Planned";
    public DateTime? PlannedStartDate { get; set; } 
    public DateTime? PlannedEndDate { get; set; }

    // acutal start - end date
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; } 
}
