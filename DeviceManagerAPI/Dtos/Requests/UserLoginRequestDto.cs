using System.ComponentModel.DataAnnotations;

namespace DevicesApp.Dtos.Requests;

public class UserLoginRequestDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
