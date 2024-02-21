using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public record CategoryRequestUpdate
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int Status { get; set; }
        public int Index { get; set; }
    }
}
