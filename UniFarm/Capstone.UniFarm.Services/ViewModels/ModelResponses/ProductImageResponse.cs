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
        public Guid ProductId { get; set; }
        public string Image { get; set; }
        public int DisplayIndex { get; set; }
    }
}
