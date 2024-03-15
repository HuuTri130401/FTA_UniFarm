using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class MenuRequestUpdate
    {
        //public Guid? FarmHubId { get; set; }
        public Guid? BusinessDayId { get; set; }
        [StringLength(50, ErrorMessage = "Name length cannot exceed 50 characters")]
        public string? Name { get; set; }
        [StringLength(20, ErrorMessage = "Tag length cannot exceed 50 characters")]
        public string? Tag { get; set; }
        [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status must be either 'Active' or 'Inactive'")]
        public string? Status { get; set; }
    }
}
