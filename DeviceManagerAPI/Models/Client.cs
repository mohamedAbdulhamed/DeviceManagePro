using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevicesApp.Models
{
    public class Client
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 14, ErrorMessage = "National ID must be between 14 and 20 characters.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "National ID must be numeric.")]
        public string NationalId { get; set; }

        // The negative sign (-) indicates whether the location is south or north of the equator (Negative values indicate locations south of the equator).
        [Required]
        [Range(-90.0, 90.0, ErrorMessage = "Latitude must be between -90 and 90 degrees.")]
        public double Latitude { get; set; }

        [Required]
        [Range(-180.0, 180.0, ErrorMessage = "Longitude must be between -180 and 180 degrees.")]
        public double Longitude { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public DateOnly UpdatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        [InverseProperty(nameof(Device.Client))]
        public ICollection<Device> Devices { get; set; }
    }
}
