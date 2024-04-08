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
}