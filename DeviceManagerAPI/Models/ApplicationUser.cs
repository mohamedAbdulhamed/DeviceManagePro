using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DevicesApp.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime RefreshTokenExpiryTime { get; set; }
}
