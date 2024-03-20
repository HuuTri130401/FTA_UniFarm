using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class ProductImageRequestUpdate
    {
        public Guid? ProductId { get; set; }
        [StringLength(50, ErrorMessage = "Caption length cannot exceed 50 characters")]
        public string? Caption { get; set; }
        public IFormFile? ImageUrl { get; set; }

        [Range(1, 5, ErrorMessage = "DisplayIndex must be between 1 and 5")]
        public int? DisplayIndex { get; set; }
        [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status must be either 'Active' or 'Inactive'")]
        public string? Status { get; set; }
    }
}
