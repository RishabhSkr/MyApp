using System.ComponentModel.DataAnnotations;
namespace BackendAPI.Dtos.Product;

public class ProductCreateDto
{
    [Required]
    [StringLength(100)]
    public string   Name { get; set; } = string.Empty;
    
    // Testing ke liye add karein (Baad mein hata dena)
    public int CreatedByUserId { get; set; }
} 