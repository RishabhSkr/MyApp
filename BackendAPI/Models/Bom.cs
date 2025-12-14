using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    public class Bom
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductID { get; set; }
        public Product Product { get; set; } = null!;

        [ForeignKey(nameof(RawMaterial))]
        public int RawMaterialId { get; set; }
        public RawMaterial? RawMaterial { get; set; } 

        public decimal QuantityRequired { get; set; } 
    }
}