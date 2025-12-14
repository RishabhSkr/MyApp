using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Models;

public class Product
{
    [Key]
    public int ProductID { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    // Navigation
    public ICollection<Bom> Boms { get; set; } = new List<Bom>();
}
