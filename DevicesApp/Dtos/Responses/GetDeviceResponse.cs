using DevicesApp.Models;
using System.ComponentModel.DataAnnotations;

namespace DevicesApp.Dtos.Responses
{
    public class GetDeviceResponse
    {
        [Required]
        public string SerialNo { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DeviceStatus Status { get; set; }

        [Required]
        public DateOnly CreatedAt { get; set; }

        [Required]
        public DateOnly UpdatedAt { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        [Required]
        public Guid TypeId { get; set; }
    }
}
