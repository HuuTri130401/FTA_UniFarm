using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public class BatchResponse
    {
        public Guid Id { get; set; }
        public Guid CollectedId { get; set; }
        public Guid FarmHubId { get; set; }
        public Guid BusinessDayId { get; set; }
        public DateTime FarmShipDate { get; set; }
        public DateTime CollectedHubReceiveDate { get; set; }
        public string ReceivedDescription { get; set; }
        public string FeedBackImage { get; set; }
        public string Status { get; set; }
        public virtual BusinessDayResponse BusinessDay { get; set; } = null!;
        public virtual CollectedHubResponse Collected { get; set; } = null!;
        public virtual FarmHubResponse FarmHub { get; set; } = null!;
        public virtual ICollection<OrderResponse> Orders { get; set; }
    }


    public record BatchResponseSimple
    {
        public Guid Id { get; set; }
        public DateTime? FarmShipDate { get; set; }
        public DateTime? CollectedHubReceiveDate { get; set; }
        public string? ReceivedDescription { get; set; }
        public string? FeedBackImage { get; set; }
        public string? Status { get; set; }
    }
}
