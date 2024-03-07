using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public class ProductImageResponse
    {
        public Guid Id { get; set; }
        public Guid ProductItemId { get; set; }
        public string Caption { get; set; }
        public string ImageUrl { get; set; }
        public int DisplayIndex { get; set; }
        public string Status { get; set; }
    }
}
