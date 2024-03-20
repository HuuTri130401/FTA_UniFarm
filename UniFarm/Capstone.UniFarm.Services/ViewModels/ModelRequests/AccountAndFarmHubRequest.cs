using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class AccountAndFarmHubRequest
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

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "The phone number must be 10 characters long.")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Invalid phone number format. 0[3|5|7|8|9] + 8 digits.")]
        public string PhoneNumber { get; set; }
        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "FarmHubName is required")]
        [StringLength(50, ErrorMessage = "FarmHubName length cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "FarmHubName length must be at least 3 characters")]
        public string FarmHubName { get; set; }

        [Required(ErrorMessage = "FarmHubCode is required")]
        [StringLength(10, ErrorMessage = "FarmHubCode length cannot exceed 10 characters")]
        [MinLength(4, ErrorMessage = "FarmHubCode length must be at least 2 characters")]
        public string FarmHubCode { get; set; }

        [StringLength(255, ErrorMessage = "Description length cannot exceed 255 characters")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "FarmHubImage is required")]
        public IFormFile FarmHubImage { get; set; }

        [Required(ErrorMessage = "FarmHubAddress is required")]
        [StringLength(100, ErrorMessage = "FarmHubAddress length cannot exceed 100 characters")]
        public string FarmHubAddress { get; set; }
    }
}
