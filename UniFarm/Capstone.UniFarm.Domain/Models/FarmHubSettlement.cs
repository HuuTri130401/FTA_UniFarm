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
    public class FarmHubSettlement
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("FarmHub")]
        public Guid FarmHubId { get; set; }

        [ForeignKey("BusinessDay")]
        public Guid BusinessDayId { get; set; }
        [ForeignKey("PriceTable")]
        public Guid PriceTableId { get; set; }
        public virtual FarmHub FarmHub { get; set; }
        public virtual BusinessDay BusinessDay { get; set; }
        public virtual PriceTable PriceTable { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalSales { get; set; } //Tổng tiền tất cả order
        public int NumOfOrder { get; set; } //Tổng số order của 1 farmhub
        [Column(TypeName = "decimal(18, 2)")]
        public decimal DeliveryFeeOfOrder { get; set; } //Phí vận chuyển 1 order

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CommissionFee { get; set; } //Phí hoa hồng lưu trữ theo danh mục sản phẩm

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DailyFee { get; set; } //Phí bậc thang quy định theo PriceTable
        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountToBePaid { get; set; } //Số tiền cần chi trả cho các loại phí 
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Profit { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
