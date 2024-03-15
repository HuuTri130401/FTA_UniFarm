using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class ProductItemRequestUpdate
    {
        public Guid? ProductId { get; set; }
        [StringLength(50, ErrorMessage = "Title length cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Title length must be at least 3 characters")]
        public string? Title { get; set; }
        [StringLength(255, ErrorMessage = "Description length cannot exceed 255 characters")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "ProductOrigin length cannot exceed 100 characters")]
        [MinLength(2, ErrorMessage = "ProductOrigin length must be at least 2 characters")]
        public string? ProductOrigin { get; set; }
        public string? SpecialTag { get; set; }
        public string? StorageType { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative number.")]
        public decimal? Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
        public double? Quantity { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "MinOrder must be a non-negative number.")]
        public double? MinOrder { get; set; }
        public string? Unit { get; set; }
        //[RegularExpression("^(Active|Inactive|Available)$", ErrorMessage = "Status must be either 'Active' or 'Inactive' or 'Available'")]
        //public string? Status { get; set; }
    }
}
