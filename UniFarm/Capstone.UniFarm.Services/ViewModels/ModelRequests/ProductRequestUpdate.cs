using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class ProductRequestUpdate
    {
        public Guid? CategoryId { get; set; }
        [StringLength(50, ErrorMessage = "Name length cannot exceed 50 characters")]
        [MinLength(2, ErrorMessage = "Name length must be at least 2 characters")]
        public string? Name { get; set; }
        [StringLength(10, ErrorMessage = "Code length cannot exceed 10 characters")]
        [MinLength(4, ErrorMessage = "Code length must be at least 4 characters")]
        public string? Code { get; set; }
        [StringLength(1000, ErrorMessage = "Description length cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [StringLength(30, ErrorMessage = "Label length cannot exceed 30 characters")]
        public string? Label { get; set; }
        [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status must be either 'Active' or 'Inactive'")]
        public string? Status { get; set; }
    }
}
