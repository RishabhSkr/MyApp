using System.ComponentModel.DataAnnotations;
namespace BackendAPI.Dtos;

public class ProductCreateDto
{
    [Required]
    [StringLength(100)]
    public string   Name { get; set; } = string.Empty;

    [Range(1,100000)]
    public decimal  Price { get; set; }
} 