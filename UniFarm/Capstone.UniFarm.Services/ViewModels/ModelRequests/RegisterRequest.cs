using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.UniFarm.Domain.Enum;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public record RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(8, ErrorMessage = "The password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#$%^&*()_+]{8,}$", ErrorMessage = "Invalid password format.")]
        public string Password { get; set; }


        [Required]
        [StringLength(10)]
        [RegularExpression(@"^(84|0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(50)]
        public string? UserName { get; set; }
        
        public RoleEnum? Role { get; set; }
    }
}
