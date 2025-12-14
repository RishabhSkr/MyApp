using System.ComponentModel.DataAnnotations;
namespace BackendAPI.Dtos.Product;

public class ProductUpdateDto
{
    [Required]
    public string   Name { get; set; } = string.Empty;

}