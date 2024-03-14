using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class ProductItemInMenuRequest
    {
        [Required(ErrorMessage = "ProductItemId is required")]
        public Guid ProductItemId { get; set; }
        //[Required(ErrorMessage = "MenuId is required")]
        //public Guid MenuId { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "SalePrice must be a non-negative number.")]
        public double SalePrice { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
        public double Quantity { get; set; }
    }
}
