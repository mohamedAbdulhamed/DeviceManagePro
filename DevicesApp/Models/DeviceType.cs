using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevicesApp.Models
{
    public class DeviceType
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [InverseProperty(nameof(Device.Type))]
        public ICollection<Device> Devices { get; set; }
    }
}
