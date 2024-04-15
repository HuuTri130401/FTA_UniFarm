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
        [FromQuery] EnumConstants.CustomerStatus? status,
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

        var result = await _orderService.GetAllOrdersOfCustomer(
            isAscending: isAscending,
            orderBy: orderBy,
            filter: x => x.CustomerId == defineUser.Payload.Id
                         && x.IsPaid == true
                         && (string.IsNullOrEmpty(status.ToString()) || x.CustomerStatus == status.ToString()),
            pageIndex: pageIndex,
            pageSize: pageSize
            );
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result.Payload);
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
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result.Payload);
    }
    
    
}