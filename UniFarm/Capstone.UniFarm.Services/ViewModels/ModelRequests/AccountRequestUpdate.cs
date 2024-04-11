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
        public string? Email { get; set; }
        public string? Password { get; set; }
        
        /*[StringLength(10, MinimumLength = 10, ErrorMessage = "The phone number must be 10 characters long.")]
        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Invalid phone number format. 0[3|5|7|8|9] + 8 digits.")]*/
        public string? PhoneNumber { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(50)]
        public string? UserName { get; set; }
        public string? Avatar { get; set; }  = null;
        public string? Code { get; set; }
        public string? Address { get; set; }
        [RegularExpression("^(Customer|FarmHub|CollectedStaff|StationStaff)$", ErrorMessage = "Status can be either Customer, FarmHub, CollectedStaff, StationStaff")]
        public string Role { get; set; }
    }
}
