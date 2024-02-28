using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public class MenuResponse
    {
        public Guid Id { get; set; }
        public Guid FarmHubId { get; set; }
        public Guid BusinessDayId { get; set; }
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Tag { get; set; }
        public string Status { get; set; }

    }
}
