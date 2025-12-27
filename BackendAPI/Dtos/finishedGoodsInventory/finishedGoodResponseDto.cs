namespace BackendAPI.Dtos.Inventory
{
    public class FinishedGoodsResponseDto
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal AvailableQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}