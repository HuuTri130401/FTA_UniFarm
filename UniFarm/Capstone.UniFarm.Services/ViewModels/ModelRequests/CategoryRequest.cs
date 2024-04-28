using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.UniFarm.Services.Commons;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public record CategoryRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name length cannot exceed 50 characters")]
        [MinLength(2, ErrorMessage = "Name length must be at least 2 characters")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description length cannot exceed 1000 characters")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Image URL is required")]
        public string Image { get; set; }

        [Required(ErrorMessage = "DisplayIndex is required")]
        [Range(1, int.MaxValue, ErrorMessage = "DisplayIndex must be greater than 0.")]
        public int DisplayIndex { get; set; }

        [Required(ErrorMessage = "SystemPrice is required")]
        [Range(0, double.MaxValue, ErrorMessage = "SystemPrice must be a non-negative number.")]
        public double SystemPrice { get; set; }
    }
}
