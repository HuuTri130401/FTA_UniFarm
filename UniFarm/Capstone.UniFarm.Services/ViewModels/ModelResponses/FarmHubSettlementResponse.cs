﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ViewModels.ModelResponses
{
    public class FarmHubSettlementResponse
    {
        public Guid Id { get; set; }
        public Guid FarmHubId { get; set; }
        public Guid BusinessDayId { get; set; }
        public string BusinessDayName { get; set; }
        public DateTime BusinessOpenday { get; set; }
        public Guid PriceTableId { get; set; }
        public decimal TotalSales { get; set; } //Tổng tiền tất cả order
        public int NumOfOrder { get; set; } //Tổng số order của 1 farmhub
        public decimal DeliveryFeeOfOrder { get; set; } //Phí vận chuyển 1 order
        public decimal CommissionFee { get; set; } //Phí hoa hồng lưu trữ theo danh mục sản phẩm
        public decimal DailyFee { get; set; } //Phí bậc thang quy định theo PriceTable
        public decimal AmountToBePaid { get; set; } //Số tiền cần chi trả cho các loại phí 
        public decimal Profit { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
