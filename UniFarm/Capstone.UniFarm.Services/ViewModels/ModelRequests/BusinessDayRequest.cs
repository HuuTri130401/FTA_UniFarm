using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class BusinessDayRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name length cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Name length must be at least 3 characters")]
        public string Name { get; set; }
        public DateTime OpenDay { get; set; }
        //public DateTime RegiterDay { get; set; }
        //public DateTime EndOfRegister { get; set; }
        //public TimeSpan StartTime { get; set; }
        //public TimeSpan EndTime { get; set; }
        //public DateTime EndOfDay { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public DateTime UpdatedAt { get; set; }
        //public string Status { get; set; }
    }
}
