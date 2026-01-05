using System.ComponentModel.DataAnnotations.Schema;
namespace BackendAPI.Models;

public class RolePermission
{
    public int Id { get; set; }

    public int RoleId { get; set; }
    [ForeignKey(nameof(RoleId))]
    public Role? Role { get; set; }
    public string RouteName { get; set; }=string.Empty;
}