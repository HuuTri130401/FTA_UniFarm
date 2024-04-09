using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IAdminDashboardService
{
    Task<OperationResult<AdminDashboardResponse.OrderStatisticByBusinessDay>> GetOrderStatisticByBusinessDay(Guid? id,
        DateTime? businessDay);

    Task<OperationResult<IEnumerable<OrderResponse.OrderResponseForStaff>>>
        GetAllOrdersOfStaff(
            bool? isAscending,
            string? orderBy,
            Expression<Func<Order, bool>>? filter = null,
            int pageIndex = 0,
            int pageSize = 10);

    Task<OperationResult<OrderResponse.OrderAndOrderDetailResponse>> GetOrderDetail(Guid businessDayId, Guid orderId);
    Task<OperationResult<IEnumerable<AdminDashboardResponse.RevenueByMonth>>> GetStatisticByMonth();
    
    Task<OperationResult<IEnumerable<AdminDashboardResponse.ProductSellingPercent>>> GetProductSellingPercent(DateTime? fromDate, DateTime? toDate);
}