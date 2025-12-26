using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace BackendAPI.Models;
public class ProductionOrder : AuditableEntity
{
    [Key]
    public int ProductionOrderId { get; set; }
    // Link to SalesOrder
    [ForeignKey(nameof(SalesOrder))]
    public int SalesOrderId { get; set; }
    public SalesOrder SalesOrder { get; set; } = null!;

    // Link to Product
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
    //  Qty
    public decimal PlannedQuantity { get; set; }
    public decimal ProducedQuantity { get; set; } = 0;
    // State -> Planned - Inprogress- Completed
    public string Status { get; set; }="Planned";

    
    public DateTime? PlannedStartDate { get; set; } 
    public DateTime? PlannedEndDate { get; set; }

    // acutal start - end date
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; } 

}
