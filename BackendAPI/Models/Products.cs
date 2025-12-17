using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; } 

    [Required]
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!; 
    
    // Navigation
    public ICollection<Bom> Boms { get; set; } = new List<Bom>();
}