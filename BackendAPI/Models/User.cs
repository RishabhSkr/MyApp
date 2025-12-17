using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(50)]
    public string? Username { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? PasswordHash { get; set; }

    // 🔑 Foreign Key
    [ForeignKey(nameof(Role))]
    public int RoleId { get; set; }

    // Navigation Property
    public Role? Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // Navigation Property 
    public ICollection<ProductionOrder> ProductionOrders { get; set; } = new List<ProductionOrder>();
}
