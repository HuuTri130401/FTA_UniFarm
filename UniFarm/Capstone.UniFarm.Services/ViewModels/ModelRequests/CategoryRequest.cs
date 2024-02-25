using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public record CategoryRequest
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; }
        public int? DisplayIndex { get; set; }
        public double? SystemPrice { get; set; }
        public double? MinSystemPrice { get; set; }
        public double? MaxSystemPrice { get; set; }
        public double? Margin { get; set; }
    }
}
