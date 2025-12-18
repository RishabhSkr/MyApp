using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models;

public class Product :AuditableEntity
{
    [Key]
    public int ProductId { get; set; } 

    [Required]
    public string Name { get; set; } = string.Empty;
    
    // Navigation
    public ICollection<Bom> Boms { get; set; } = new List<Bom>();
}