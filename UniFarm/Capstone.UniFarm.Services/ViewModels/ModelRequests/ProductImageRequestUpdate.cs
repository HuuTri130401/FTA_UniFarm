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
        public IFormFile? ImageUrl { get; set; }
    }
}
