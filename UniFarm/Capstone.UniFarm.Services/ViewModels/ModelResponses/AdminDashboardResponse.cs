namespace Capstone.UniFarm.Services.ViewModels.ModelResponses;

public record AdminDashboardResponse
{
    
    
   /*- Tổng doanh thu của tất cả các đơn hàng 
    - Tổng số đơn hàng 
    - Tổng số đơn hàng đang giao
    - Tổng số đơn hàng thành công
    - Tổng số đơn hàng bị hủy bởi khách hàng
    - Tổng số đơn hàng bị hủy bởi farm
    - Tổng số đơn hàng bị hủy bới hệ thống
    - Tổng số đơn hàng hết hạn
    - Tổng số đơn hàng đang chờ xác nhận*/
    public record OrderStatisticByBusinessDay
    {
        public decimal TotalRevenue { get; init; }
        public int TotalOrder { get; init; }
        public int TotalOrderDelivering { get; init; }
        public int TotalOrderSuccess { get; init; }
        public int TotalOrderCancelByCustomer { get; init; }
        public int TotalOrderCancelByFarm { get; init; }
        public int TotalOrderCancelBySystem { get; init; }
        public int TotalOrderExpired { get; init; }
        public int TotalOrderPending { get; init; }
        public int TotalOrderConfirmed { get; init; }
    }

    public record DashboardStatistic
    {
        public List<RevenueByMonth>? RevenueByMonths { get; init; }
        public List<ProductSellingPercent>? ProductSellingPercents { get; init; }
    }
    
    
    public record RevenueByMonth
    {
        public string? Month { get; set; }
        public decimal? TotalRevenue { get; set; }
        public decimal? TotalDepositMoney { get; set; }
        public decimal? TotalWithdrawMoney { get; set; }
        public decimal? TotalRefundMoney { get; set; }
        public decimal? TotalBenefit { get; set; }
        
        public int? TotalOrder { get; set; }
        public int? TotalOrderSuccess { get; set; }
        public int? TotalOrderCancel { get; set; }
        public int? TotalOrderExpired { get; set; }
    }
    
    public record ProductSellingPercent
    {
        public Guid ProductItemId { get; set; }
        public string? ProductName { get; set; }
        public double SoldQuantity { get; set; }
        public double Percent { get; set; }
    }
    
    public record ProductItem
    {
        public Guid? Id { get; init; }
        public string? Name { get; init; }
        public decimal? Price { get; init; }
        public int? Quantity { get; init; }
        public string? Unit { get; init; }
        public decimal? TotalRevenue { get; init; }
        public int? TotalOrderCancel { get; init; }
        public int? TotalOrderSuccess { get; init; }
    }
    
    public record TopFarmHub
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public Guid? OwnerId { get; set; }
        public string? OwnerName { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }
        public decimal? TotalRevenue { get; set; }
        public int? TotalOrderCancel { get; set; }
        public int? TotalOrderSuccess { get; set; }
    }
}