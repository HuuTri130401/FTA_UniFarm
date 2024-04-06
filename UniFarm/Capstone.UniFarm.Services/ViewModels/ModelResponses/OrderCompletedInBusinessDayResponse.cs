using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public class OrderCompletedInBusinessDayResponse
    {

    }

    public class OrderDetailCompleted
    {
        public Guid OrderId { get; set; }
        public Guid ProductItemId { get; set; }
        public double Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OriginUnitPrice { get; set; }
        public string Unit { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalOriginPrice { get; set; }
    }
}
