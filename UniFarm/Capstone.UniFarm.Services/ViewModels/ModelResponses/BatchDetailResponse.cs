using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public class BatchDetailResponse
    {
        public Guid Id { get; set; }
        public Guid CollectedId { get; set; }
        public string CollectedHubName { get; set; }
        public Guid FarmHubId { get; set; }
        public string FarmHubName { get; set; }
        public Guid BusinessDayId { get; set; }
        public string BusinessDayName { get; set; }
        public string BusinessDayOpen { get; set; }
        public int? NumberOfOrdersInBatch { get; set; }
        public DateTime? FarmShipDate { get; set; }
        public DateTime? CollectedHubReceiveDate { get; set; }
        public string? ReceivedDescription { get; set; }
        public string? FeedBackImage { get; set; }
        public string Status { get; set; }
        public virtual ICollection<OrdersInBatchResponse> Orders { get; set; }
    }
}
