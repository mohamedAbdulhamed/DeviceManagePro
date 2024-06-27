using System.ComponentModel.DataAnnotations;

namespace DevicesApp.Dtos.Responses
{
    public class GetClientResponse
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20, MinimumLength = 14, ErrorMessage = "National ID must be between 14 and 20 characters.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "National ID must be numeric.")]
        public string NationalId { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        [Required]
        public DateOnly CreatedAt { get; set; }

        [Required]
        public DateOnly UpdatedAt { get; set; }
    }
}
