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
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative number.")]
        public decimal? Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
        public double? Quantity { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "MinOrder must be a non-negative number.")]
        public double? MinOrder { get; set; }
        public string? Unit { get; set; }
        [RegularExpression("^(Active|InActive)$", ErrorMessage = "Status must be either 'Active' or 'InActive'")]
        public string? Status { get; set; }
    }
}
