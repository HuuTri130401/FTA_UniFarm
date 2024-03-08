using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public class ProductItemInMenuResponse
    {
        public Guid Id { get; set; }
        public Guid ProductItemId { get; set; }
        public Guid MenuId { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }
        public MenuResponse? Menu { get; set; }
        public ProductItemResponse? ProductItem { get; set; }

    }
}
