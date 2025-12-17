using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models;

public class Bom
{
    [Key]
    public int Id { get; set; }
    
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }

    public int RawMaterialId { get; set; }
    
    [ForeignKey(nameof(RawMaterialId))]
    public RawMaterial? RawMaterial { get; set; }

    [Required]
    public decimal QuantityRequired { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int CreatedByUserId { get; set; }
    
    [ForeignKey(nameof(CreatedByUserId))]
    public User? CreatedByUser { get; set; }
}