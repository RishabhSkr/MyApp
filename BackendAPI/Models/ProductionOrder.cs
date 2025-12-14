using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using BackendAPI.Models;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
public class ProductionOrder
{
    [Key]
    public int ProductionOrderID { get; set; }
    // Link to SalesOrder
    [ForeignKey(nameof(SalesOrder))]
    public int SalesOrderID { get; set; }
    public SalesOrder SalesOrder { get; set; } = null!;

    // Link to Product
    public int ProductID { get; set; }
    [ForeignKey("ProductID")]
    public virtual Product? Product { get; set; }
    //  Qty
    public decimal PlannedQuantity { get; set; }
    public decimal ProducedQuantity { get; set; } = 0;
    // State -> Planned - Inprogress- Completed
    public string Status { get; set; }="Planned";
    
    // Link to User
    public int CreatedByUserId { get; set; }
    [ForeignKey(nameof(CreatedByUserId))]
    public virtual User? CreatedByUser { get; set; }

    public DateTime? StartDate { get; set; } 
    public DateTime? CompletedDate { get; set; }
}