using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public class OrderResponseForFarmHub
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid BusinessDayId { get; set; }
        public string BusinessDayName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Code { get; set; }
        public string ShipAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomerStatus { get; set; }
        public string DeliveryStatus { get; set; }
        public bool IsPaid { get; set; }
    }
}
