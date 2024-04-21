using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public class BusinessDayResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime RegiterDay { get; set; }
        public DateTime EndOfRegister { get; set; }
        public DateTime OpenDay { get; set; }
        public DateTime StopSellingDay { get; set; }
        public DateTime EndOfDay { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; }
        public ICollection<MenuResponse> Menus { get; set; }
    }
}
