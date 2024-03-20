using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public record CategoryRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name length cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Name length must be at least 3 characters")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "Description length cannot exceed 255 characters")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Image URL is required")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Code is required")]
        [StringLength(10, ErrorMessage = "Code length cannot exceed 10 characters")]
        [MinLength(4, ErrorMessage = "Code length must be at least 4 characters")]
        public string Code { get; set; }

        [Required(ErrorMessage = "DisplayIndex is required")]
        [Range(1, int.MaxValue, ErrorMessage = "DisplayIndex must be greater than 0.")]
        public int DisplayIndex { get; set; }

        [Required(ErrorMessage = "SystemPrice is required")]
        [Range(0, double.MaxValue, ErrorMessage = "SystemPrice must be a non-negative number.")]
        public double SystemPrice { get; set; }

        [Required(ErrorMessage = "MinSystemPrice is required")]
        [Range(0, double.MaxValue, ErrorMessage = "MinSystemPrice must be a non-negative number.")]
        public double MinSystemPrice { get; set; }

        [Required(ErrorMessage = "MaxSystemPrice is required")]
        [Range(0, double.MaxValue, ErrorMessage = "MaxSystemPrice must be a non-negative number.")]
        public double MaxSystemPrice { get; set; }

        [Required(ErrorMessage = "Margin is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Margin must be a non-negative number.")]
        public double Margin { get; set; }
    }
}
