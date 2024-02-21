using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class ProductRequest
    {
        public Guid FarmHubId { get; set; }
        public Guid CategoryId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Label { get; set; }
        public string SpecialTag { get; set; }
        public string Source { get; set; }
    }
}
