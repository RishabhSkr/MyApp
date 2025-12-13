// Models/Product.cs
using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Models
{
    // This Class becomes a Table in SQL Server automatically!
    public class Product
    {
        [Key] // This marks it as the Primary Key
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        
        public string Sku { get; set; } = string.Empty;

        // "RawMaterial" or "FinishedGood"
        public string Type { get; set; } = "RawMaterial"; 

        public decimal Price { get; set; }
        
        public int StockQuantity { get; set; }
    }
}