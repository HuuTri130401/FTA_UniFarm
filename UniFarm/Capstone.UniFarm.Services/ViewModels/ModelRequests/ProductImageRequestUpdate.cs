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
        [StringLength(255, ErrorMessage = "Image URL length cannot exceed 255 characters")]
        public string? Image { get; set; }
        [Range(1, 5, ErrorMessage = "DisplayIndex must be between 1 and 5")]
        public int? DisplayIndex { get; set; }
    }
}
