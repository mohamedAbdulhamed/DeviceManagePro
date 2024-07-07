using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DevicesApp.Models
{
    public class Device
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string SerialNo { get; set; }

        [Required]
        public string Name { get; set; }

        public DeviceStatus Status { get; set; } = DeviceStatus.OFF;

        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public DateOnly UpdatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        [Required]
        public Guid CreatedBy { get; set; }

        [Required]
        public Guid TypeId { get; set; }

        [Required]
        [ForeignKey(nameof(TypeId))]
        public DeviceType Type { get; set; }

        public Guid? ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }
    }

    public enum DeviceStatus
    {
        OFF = 0,
        ON = 1,
    }
}
