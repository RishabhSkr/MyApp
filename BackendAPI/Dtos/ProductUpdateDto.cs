using System.ComponentModel.DataAnnotations;
namespace BackendAPI.Dtos;

public class ProductUpdateDto
{
    [Required]
    public string   Name { get; set; } = string.Empty;

    [Required]
    public decimal  Price { get; set; }
} 