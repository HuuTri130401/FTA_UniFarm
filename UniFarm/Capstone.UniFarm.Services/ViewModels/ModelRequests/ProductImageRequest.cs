using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class ProductImageRequest
    {
        [Required(ErrorMessage = "Caption is required")]
        [StringLength(50, ErrorMessage = "Caption length cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Caption length must be at least 3 characters")]
        public string Caption { get; set; } 
        [Required(ErrorMessage = "Image URL is required")]
        public IFormFile ImageUrl { get; set; }
        [Required(ErrorMessage = "DisplayIndex is required")]
        [Range(1, 5, ErrorMessage = "DisplayIndex must be between 1 and 5")]
        public int DisplayIndex { get; set; }
    }
}
