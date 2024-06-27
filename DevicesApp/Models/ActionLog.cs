using System.ComponentModel.DataAnnotations;

namespace DevicesApp.Models;

public class ActionLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string UserId { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    public string UserRole { get; set; }

    [Required]
    public string Action { get; set; }

    [Required]
    public string Entity { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}