using Capstone.UniFarm.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelRequests
{
    public class PriceTableItemRequestUpdate
    {
        [Range(0, double.MaxValue, ErrorMessage = "FromAmount must be a non-negative number.")]
        public decimal? FromAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "ToAmount must be a non-negative number.")]
        public decimal? ToAmount { get; set; }

        [Range(0, 100, ErrorMessage = "Percentage must be a non-negative number and cannot exceed 100.")]
        public decimal? Percentage { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MinFee must be a non-negative number.")]
        public decimal? MinFee { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MaxFee must be a non-negative number.")]
        public decimal? MaxFee { get; set; }
    }
}
