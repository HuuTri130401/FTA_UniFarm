using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class FarmHubRequestUpdate
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string Address { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; }
    }
}
