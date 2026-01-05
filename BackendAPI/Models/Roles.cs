using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <-- YEH MISSING HAI
namespace BackendAPI.Models;

public class Role
{
    [Key]
    public int RoleId { get; set; }

    [Required]
    [MaxLength(50)]
    public string? RoleName { get; set; }
    // Navigation Property
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
