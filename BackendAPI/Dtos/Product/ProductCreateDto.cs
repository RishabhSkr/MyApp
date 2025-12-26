using System.ComponentModel.DataAnnotations;
namespace BackendAPI.Dtos.Product;

public class ProductCreateDto
{
    [Required]
    [StringLength(100)]
    public string   Name { get; set; } = string.Empty;
    
    public int? maxDailyCapacity { get; set; }
} 