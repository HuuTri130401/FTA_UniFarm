using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class OrderCustomerController : BaseController
{
    private readonly IOrderService _orderService;
    private readonly IAccountService _accountService;

    public OrderCustomerController(IOrderService orderService, IAccountService accountService)
    {
        _orderService = orderService;
        _accountService = accountService;
    }

    [HttpPost("order/create")]
    [Authorize]
    [SwaggerOperation(Summary = "Create order - Tien")]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequestCreate orderRequestCreate)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);

        var result = await _orderService.CreateOrder(orderRequestCreate, defineUser.Payload.Id);
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result.Payload);
    }
    
    [HttpPost("order/Checkout")]
    [Authorize]
    [SwaggerOperation(Summary = "Create order - Tien")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest orderRequestCreate)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);
        var result = await _orderService.Checkout(defineUser.Payload.Id, orderRequestCreate);
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result.Payload);
    }
    
    

    [HttpGet("orders/get-all")]
    [Authorize(Roles = "Customer")]
    [SwaggerOperation(Summary = "Get all order of customer - Tien")]
    public async Task<IActionResult> GetAllOrder(
        [FromQuery] EnumConstants.FilterOrderStatus? status,
        [FromQuery] bool? isAscending = false,
        [FromQuery] string? orderBy = "CreatedAt",
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10
    )
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);

        if (status != null)
        {
            if (status == EnumConstants.FilterOrderStatus.Pending)
            {
                var resultPending = await _orderService.GetAllOrdersOfCustomer(
                    isAscending: isAscending,
                    orderBy: orderBy,
                    filter: x => x.CustomerId == defineUser.Payload.Id
                                 && x.CustomerStatus == status.ToString(),
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
                return resultPending.IsError ? HandleErrorResponse(resultPending.Errors) : Ok(resultPending);
            } else if (status == EnumConstants.FilterOrderStatus.Confirmed)
            {
                var resultConfirmed = await _orderService.GetAllOrdersOfCustomer(
                    isAscending: isAscending,
                    orderBy: orderBy,
                    filter: x => x.CustomerId == defineUser.Payload.Id
                                 && x.CustomerStatus == status.ToString(),
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
                return resultConfirmed.IsError ? HandleErrorResponse(resultConfirmed.Errors) : Ok(resultConfirmed);
            } else if (status == EnumConstants.FilterOrderStatus.OnDelivery)
            {
                var resultShipping = await _orderService.GetAllOrdersOfCustomer(
                    isAscending: isAscending,
                    orderBy: orderBy,
                    filter: x => x.CustomerId == defineUser.Payload.Id
                                 && (x.DeliveryStatus == EnumConstants.DeliveryStatus.OnTheWayToCollectedHub.ToString()
                                 || x.DeliveryStatus == EnumConstants.DeliveryStatus.AtCollectedHub.ToString()
                                 || x.DeliveryStatus == EnumConstants.DeliveryStatus.OnTheWayToStation.ToString()
                                     ),
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
                return resultShipping.IsError ? HandleErrorResponse(resultShipping.Errors) : Ok(resultShipping);
            }
            else if (status == EnumConstants.FilterOrderStatus.ReadyForPickup)
            {
                var resultDelivered = await _orderService.GetAllOrdersOfCustomer(
                    isAscending: isAscending,
                    orderBy: orderBy,
                    filter: x => x.CustomerId == defineUser.Payload.Id
                                 && (x.CustomerStatus == status.ToString()
                                     || x.DeliveryStatus == EnumConstants.DeliveryStatus.AtStation.ToString()),
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
                return resultDelivered.IsError ? HandleErrorResponse(resultDelivered.Errors) : Ok(resultDelivered);
            }
            else if (status == EnumConstants.FilterOrderStatus.PickedUp)
            {
                var resultCancelled = await _orderService.GetAllOrdersOfCustomer(
                    isAscending: isAscending,
                    orderBy: orderBy,
                    filter: x => x.CustomerId == defineUser.Payload.Id
                                 && x.CustomerStatus == status.ToString(),
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
                return resultCancelled.IsError ? HandleErrorResponse(resultCancelled.Errors) : Ok(resultCancelled);
                
            } else if (status == EnumConstants.FilterOrderStatus.Expired)
            {
                var resultExpired = await _orderService.GetAllOrdersOfCustomer(
                    isAscending: isAscending,
                    orderBy: orderBy,
                    filter: x => x.CustomerId == defineUser.Payload.Id
                                 && x.CustomerStatus == status.ToString(),
                    pageIndex: pageIndex,
                    pageSize: pageSize
                );
                return resultExpired.IsError ? HandleErrorResponse(resultExpired.Errors) : Ok(resultExpired);
            }
            var result = await _orderService.GetAllOrdersOfCustomer(
                isAscending: isAscending,
                orderBy: orderBy,
                filter: x => x.CustomerId == defineUser.Payload.Id
                             && x.CustomerStatus == status.ToString(),
                pageIndex: pageIndex,
                pageSize: pageSize
            );
            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
        }
        

        var resultAll = await _orderService.GetAllOrdersOfCustomer(
            isAscending: isAscending,
            orderBy: orderBy,
            filter: x => x.CustomerId == defineUser.Payload.Id
                         && x.IsPaid == true,
            pageIndex: pageIndex,
            pageSize: pageSize
            );
        return resultAll.IsError ? HandleErrorResponse(resultAll.Errors) : Ok(resultAll);
    }
    
    // cancel order
    [HttpPut("order/cancel/{orderId}")]
    [Authorize(Roles = "Customer")]
    [SwaggerOperation(Summary = "Cancel order - Tien")]
    public async Task<IActionResult> CancelOrder(Guid orderId)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);

        var result = await _orderService.CancelOrderByCustomer(orderId,defineUser.Payload.Id);
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
    }
    
    // tracking order
    [HttpGet("order/tracking/{orderId}")]
    [Authorize(Roles = "Customer")]
    [SwaggerOperation(Summary = "Tracking order - Tien")]
    public async Task<IActionResult> TrackingOrder(Guid orderId)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);

        var result = await _orderService.TrackingOrder(orderId, defineUser.Payload.Id);
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
    }
}