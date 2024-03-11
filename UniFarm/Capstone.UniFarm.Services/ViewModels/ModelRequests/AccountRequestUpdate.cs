using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.UniFarm.Domain.Enum;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public record AccountRequestUpdate
    {
        public Guid Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8 , ErrorMessage = "The password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d!@#$%^&*()_+]{8,}$", ErrorMessage = "Invalid password format.")]
        public string Password { get; set; }
        
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "The phone number must be 10 characters long.")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Invalid phone number format. 0[3|5|7|8|9] + 8 digits.")]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(50)]
        [Required]
        public string UserName { get; set; }
        public string? Avatar { get; set; }  = null;
        public string? Code { get; set; }
        public string? Address { get; set; }
        [RegularExpression("^(Customer|FarmHub|CollectedStaff|StationStaff)$", ErrorMessage = "Status can be either Customer, FarmHub, CollectedStaff, StationStaff")]
        public string Role { get; set; }
        public string Status { get; set; } = EnumConstants.ActiveInactiveEnum.ACTIVE;
    }
}
