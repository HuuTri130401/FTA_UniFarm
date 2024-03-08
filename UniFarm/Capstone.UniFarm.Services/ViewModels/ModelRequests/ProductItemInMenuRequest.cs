using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        //public double? Price { get; set; }
    }
}
