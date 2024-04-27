using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class PriceTableRequestUpdate
    {
        [StringLength(50, ErrorMessage = "Name length cannot exceed 50 characters")]
        [MinLength(3, ErrorMessage = "Name length must be at least 3 characters")]
        public string? Name { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
