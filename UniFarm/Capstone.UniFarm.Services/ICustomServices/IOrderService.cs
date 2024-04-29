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

    Task<OperationResult<IEnumerable<OrderResponse.OrderResponseForStaff?>?>> UpdateOrderStatusByStationStaff(UpdateOrderStatus.UpdateOrderStatusByTransfer request, AboutMeResponse.AboutMeRoleAndID defineUserPayload);
    
    Task<OperationResult<IEnumerable<OrderResponse.OrderResponseForCustomer>>>
        GetAllOrdersOfCustomer(
            bool? isAscending, 
            string? orderBy, 
            Expression<Func<Order, bool>>? filter = null,
            int pageIndex = 0, 
            int pageSize = 10);
    
    Task<OperationResult<IEnumerable<OrderResponse.OrderResponseForCustomer?>?>> Checkout(Guid customerId, CreateOrderRequest request);
    Task<OperationResult<OrderResponse.OrderResponseForCustomer>> CancelOrderByCustomer(Guid orderId, Guid payloadId);
    Task<OperationResult<IEnumerable<TrackingOrderResponse>>> TrackingOrder(Guid orderId, Guid payloadId);
    Task<Order?> GetOrderById(Guid orderId);
}