﻿using System.ComponentModel.DataAnnotations;

namespace DevicesApp.Dtos.Requests
{
    public class UserRegistrationRequestDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
