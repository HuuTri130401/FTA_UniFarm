using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public class BatchResponse
    {
        public Guid Id { get; set; }
        public Guid BusinessDayId { get; set; }
        public string BusinessDayName { get; set; }
        public string BusinessDayOpen { get; set; }
        public DateTime FarmShipDate { get; set; }
        public string FarmHubName { get; set; }
        public string CollectedHubName { get; set; }
        public string CollectedHubAddress { get; set; }
        public DateTime CollectedHubReceiveDate { get; set; }
        public string ReceivedDescription { get; set; }
        public string FeedBackImage { get; set; }
        public string Status { get; set; }
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
