﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class FarmHubRegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#$%^&*()_+]{8,}$", ErrorMessage = "Invalid password format.")]
        public string Password { get; set; }
        [StringLength(50)]
        public string? UserName { get; set; }
        [RegularExpression("^(FarmHub)$", ErrorMessage = "Role can be either FarmHub")]
        public string Role { get; set; }
    }
}
