using System.Linq.Expressions;
using Capstone.UniFarm.Domain.Models;
using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;

namespace Capstone.UniFarm.Services.ICustomServices;

public interface IOrderService
{
    
    // Customer
    Task<OperationResult<OrderRequestCreate>> CreateOrder(OrderRequestCreate orderRequestCreate, Guid createdBy);
    
    
    Task<OperationResult<IEnumerable<OrderResponse.OrderResponseForStaff>>> 
        GetAllOrdersOfStaff(
            bool? isAscending, 
            string? orderBy, 
            Expression<Func<Order, bool>>? filter = null,
            int pageIndex = 0, 
            int pageSize = 10);

    Task<OperationResult<OrderResponse.OrderResponseForStaff?>> UpdateOrderStatusByStationStaff(UpdateOrderStatus.UpdateOrderStatusByTransfer request, AboutMeResponse.AboutMeRoleAndID defineUserPayload);
    
    
}