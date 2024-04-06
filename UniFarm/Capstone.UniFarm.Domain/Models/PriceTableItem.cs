using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Domain.Models
{
    public class PriceTableItem
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("PriceTable")]
        public Guid PriceTableId { get; set; }
        public PriceTable PriceTable { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal FromAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ToAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Percentage { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MinFee { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MaxFee { get; set; }
    }
}
