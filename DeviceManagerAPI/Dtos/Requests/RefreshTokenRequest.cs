using System.ComponentModel.DataAnnotations;

namespace DevicesApp.Dtos.Requests;

public class RefreshTokenRequest
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string RefreshToken { get; set; }
}
