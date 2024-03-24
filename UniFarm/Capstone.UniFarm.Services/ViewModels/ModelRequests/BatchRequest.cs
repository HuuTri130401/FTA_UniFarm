using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class BatchRequest
    {
        [Required(ErrorMessage = "CollectedId is required")]
        public Guid CollectedId { get; set; }
        //[Required(ErrorMessage = "FarmHubId is required")]
        //public Guid FarmHubId { get; set; }
        [Required(ErrorMessage = "BusinessDayId is required")]
        public Guid BusinessDayId { get; set; }
        //public DateTime? FarmShipDate { get; set; }


        //public DateTime? CollectedHubReceiveDate { get; set; }
        //public string ReceivedDescription { get; set; }
        //public string FeedBackImage { get; set; }
    }
}
