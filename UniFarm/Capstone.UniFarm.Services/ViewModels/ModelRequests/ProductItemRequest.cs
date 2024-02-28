using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class ProductItemRequest
    {
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative number.")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
        public double Quantity { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "MinOrder must be a non-negative number.")]
        public double MinOrder { get; set; }
        [Required(ErrorMessage = "Unit is required")]
        public string Unit { get; set; }
    }
}
