﻿using System.ComponentModel.DataAnnotations;

namespace HighScore.Models
{
    public class CreateViewModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords are not same")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Phone { get; set; } = string.Empty;

    }
}
