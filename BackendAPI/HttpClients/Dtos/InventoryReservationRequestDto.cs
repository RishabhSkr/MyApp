namespace BackendAPI.HttpClients.Dtos;

// Reserve ya Return request
public class InventoryReservationRequest
{
    public Guid ProductionOrderId { get; set; }
    public string MovementType { get; set; } = string.Empty; // RESERVE, CANCEL_RETURN, UNUSED_RETURN
    public List<MaterialItem> Items { get; set; } = new();
}

public class MaterialItem
{
    public Guid RawMaterialId { get; set; }
    public decimal Quantity { get; set; }
}

// Finished Goods add request
public class FinishedGoodsRequest
{
    public Guid ProductionOrderId { get; set; }
    public Guid ProductId { get; set; }
    public decimal ProducedQuantity { get; set; }
    public decimal ScrapQuantity { get; set; }
    public string MovementType { get; set; } = "PRODUCTION";
}