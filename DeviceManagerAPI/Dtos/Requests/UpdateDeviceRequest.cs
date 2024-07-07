using DevicesApp.Models;
using System.ComponentModel.DataAnnotations;

namespace DevicesApp.Dtos.Requests
{
    public class UpdateDeviceRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string SerialNo { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Guid TypeId { get; set; }

    }
}
