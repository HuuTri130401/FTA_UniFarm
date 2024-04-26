using Capstone.UniFarm.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public class PriceTableResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public virtual ICollection<PriceTableItemResponse> PriceTableItems { get; set; }
    }
}
