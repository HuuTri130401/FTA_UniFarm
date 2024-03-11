using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public record CategoryRequestUpdate
    {
        [StringLength(50, ErrorMessage = "Name length cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Name length must be at least 3 characters")]
        public string? Name { get; set; }
        [StringLength(255, ErrorMessage = "Description length cannot exceed 255 characters")]
        public string? Description { get; set; }
        public string? Image { get; set; }
        [StringLength(10, ErrorMessage = "Code length cannot exceed 10 characters")]
        [MinLength(2, ErrorMessage = "Code length must be at least 2 characters")]
        public string? Code { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "DisplayIndex must be greater than or equal to 0")]
        public int? DisplayIndex { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "SystemPrice must be a non-negative number.")]
        public double? SystemPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MinSystemPrice must be a non-negative number.")]
        public double? MinSystemPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MaxSystemPrice must be a non-negative number.")]
        public double? MaxSystemPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Margin must be a non-negative number.")]
        public double? Margin { get; set; }
        [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status must be either 'Active' or 'Inactive'")]
        public string? Status { get; set; }
    }
}
