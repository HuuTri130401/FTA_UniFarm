using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Capstone.UniFarm.Domain.Enum.EnumConstants;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class BatchRequestUpdate
    {
        public string? ReceivedDescription { get; set; }
        public IFormFile? FeedBackImage { get; set; }
        [Required(ErrorMessage = "Status is required")]
        public CollectedHubUpdateBatch Status { get; set; }
    }
}
