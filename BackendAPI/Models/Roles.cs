using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Models;

public class Role
{
    [Key]
    public int RoleID { get; set; }

    [Required]
    [MaxLength(50)]
    public string? RoleName { get; set; }

    // Navigation Property
    public ICollection<User> Users { get; set; } = new List<User>(); //NullReferenceException
}
