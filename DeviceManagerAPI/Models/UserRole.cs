using System.ComponentModel.DataAnnotations;

namespace DeviceApp.Models;

public class UserRole
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Role { get; set; }
}
