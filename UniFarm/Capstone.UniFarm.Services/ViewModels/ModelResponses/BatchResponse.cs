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
        public Guid BusinessDayId { get; set; }
        public string BusinessDayName { get; set; }
        public string BusinessDayOpen { get; set; }
        public DateTime FarmShipDate { get; set; }
        public DateTime CollectedHubReceiveDate { get; set; }
        public string Status { get; set; }
    }
}
