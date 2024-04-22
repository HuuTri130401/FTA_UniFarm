using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class OrderForStaffController : BaseController
{
    private readonly IOrderService _orderService;
    private readonly IAccountService _accountService;

    public OrderForStaffController(IOrderService orderService, IAccountService accountService)
    {
        _orderService = orderService;
        _accountService = accountService;
    }


    [HttpGet]
    [Route("admin/orders")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Get all orders - Admin Role - Tien")]
    public async Task<IActionResult> GetAllOrdersForAdmin(
        [FromQuery] string? keyword,
        [FromQuery] Guid? stationId,
        [FromQuery] Guid? transferId,
        [FromQuery] Guid? collectedHubId,
        [FromQuery] EnumConstants.CustomerStatus? customerStatus,
        [FromQuery] EnumConstants.DeliveryStatus? deliveryStatus,
        [FromQuery] EnumConstants.FilterOrder? orderBy = EnumConstants.FilterOrder.CreatedAt,
        [FromQuery] bool isAscending = false,
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


        var result = await _orderService.GetAllOrdersOfStaff(
            isAscending: isAscending,
            orderBy: orderBy!.Value.ToString(),
            filter: x => (!stationId.HasValue || x.StationId == stationId) &&
                         (!collectedHubId.HasValue || x.CollectedHubId == collectedHubId) &&
                         (!transferId.HasValue || x.TransferId == transferId) &&
                         (string.IsNullOrEmpty(customerStatus.ToString()) ||
                          x.CustomerStatus!.Contains(customerStatus.ToString()!)) &&
                         (string.IsNullOrEmpty(deliveryStatus.ToString()) ||
                          x.DeliveryStatus!.Contains(deliveryStatus.ToString()!)) &&
                         x.IsPaid == true,
            pageIndex: pageIndex,
            pageSize: pageSize);
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
    }


    [HttpGet]
    [Route("collect-hub/orders")]
    [Authorize(Roles = "CollectedStaff, Admin")]
    [SwaggerOperation(Summary = "Get all orders of a collected hub - CollectedStaff , Admin Role - Tien")]
    public async Task<IActionResult> GetAllOrdersOfACollectedHub(
        [FromQuery] string? keyword,
        [FromQuery] Guid? stationId,
        [FromQuery] Guid? transferId,
        [FromQuery] Guid? collectedHubId,
        [FromQuery] EnumConstants.CustomerStatus? customerStatus,
        [FromQuery] EnumConstants.DeliveryStatus? deliveryStatus,
        [FromQuery] EnumConstants.FilterOrder? orderBy = EnumConstants.FilterOrder.CreatedAt,
        [FromQuery] bool isAscending = false,
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

        if (defineUser.Payload.Role == EnumConstants.RoleEnumString.ADMIN)
        {
            var result = await _orderService.GetAllOrdersOfStaff(
                isAscending: isAscending,
                orderBy: orderBy!.Value.ToString(),
                filter: x => (!stationId.HasValue || x.StationId == stationId) &&
                             (!collectedHubId.HasValue || x.CollectedHubId == collectedHubId) &&
                             (!transferId.HasValue || x.TransferId == transferId) &&
                             (string.IsNullOrEmpty(customerStatus.ToString()) ||
                              x.CustomerStatus!.Contains(customerStatus.ToString()!)) &&
                             (string.IsNullOrEmpty(deliveryStatus.ToString()) ||
                              x.DeliveryStatus!.Contains(deliveryStatus.ToString()!)) &&
                             x.IsPaid == true,
                pageIndex: pageIndex,
                pageSize: pageSize);
            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
        }
        else
        {
            var result = await _orderService.GetAllOrdersOfStaff(
                isAscending: isAscending,
                orderBy: orderBy!.Value.ToString(),
                filter: x => (x.CollectedHubId.ToString() == defineUser.Payload.AuthorizationDecision) &&
                             (!stationId.HasValue || x.StationId == stationId) &&
                             (!transferId.HasValue || x.TransferId == transferId) &&
                             (string.IsNullOrEmpty(customerStatus.ToString()) ||
                              x.CustomerStatus!.Contains(customerStatus.ToString()!)) &&
                             (string.IsNullOrEmpty(deliveryStatus.ToString()) ||
                              x.DeliveryStatus!.Contains(deliveryStatus.ToString()!)) &&
                             x.IsPaid == true,
                pageIndex: pageIndex,
                pageSize: pageSize);
            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
        }
    }


    [HttpGet]
    [Route("station/orders")]
    [Authorize(Roles = "StationStaff, Admin")]
    [SwaggerOperation(Summary = "Get all orders of a station - Station, Admin Role - Tien")]
    public async Task<IActionResult> GetAllOrdersOfStation(
        [FromQuery] string? keyword,
        [FromQuery] Guid? stationId,
        [FromQuery] Guid? transferId,
        [FromQuery] Guid? collectedHubId,
        [FromQuery] EnumConstants.CustomerStatus? customerStatus,
        [FromQuery] EnumConstants.DeliveryStatus? deliveryStatus,
        [FromQuery] EnumConstants.FilterOrder? orderBy = EnumConstants.FilterOrder.CreatedAt,
        [FromQuery] bool? isAscending = false,
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

        if (defineUser.Payload.Role == EnumConstants.RoleEnumString.ADMIN)
        {
            var result = await _orderService.GetAllOrdersOfStaff(
                isAscending: isAscending,
                orderBy: orderBy!.Value.ToString(),
                filter: x => (!stationId.HasValue || x.StationId == stationId) &&
                             (!collectedHubId.HasValue || x.CollectedHubId == collectedHubId) &&
                             (!transferId.HasValue || x.TransferId == transferId) &&
                             (string.IsNullOrEmpty(customerStatus.ToString()) ||
                              x.CustomerStatus!.Contains(customerStatus.ToString()!)) &&
                             (string.IsNullOrEmpty(deliveryStatus.ToString()) ||
                              x.DeliveryStatus!.Contains(deliveryStatus.ToString()!)) &&
                             x.IsPaid == true,
                pageIndex: pageIndex,
                pageSize: pageSize);
            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
        }
        else
        {
            var result = await _orderService.GetAllOrdersOfStaff(
                isAscending: isAscending,
                orderBy: orderBy!.Value.ToString(),
                filter: x => (x.StationId.ToString() == defineUser.Payload.AuthorizationDecision) &&
                             (!collectedHubId.HasValue || x.CollectedHubId == collectedHubId) &&
                             (!transferId.HasValue || x.TransferId == transferId) &&
                             (string.IsNullOrEmpty(customerStatus.ToString()) ||
                              x.CustomerStatus!.Contains(customerStatus.ToString()!)) &&
                             (string.IsNullOrEmpty(deliveryStatus.ToString()) ||
                              x.DeliveryStatus!.Contains(deliveryStatus.ToString()!)) &&
                             x.IsPaid == true,
                pageIndex: pageIndex,
                pageSize: pageSize);
            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
        }
    }

    [HttpGet]
    [Route("station/orders-contain-transfer")]
    [Authorize(Roles = "StationStaff, Admin")]
    [SwaggerOperation(Summary = "Get all orders of a station - Station, Admin Role - Tien")]
    public async Task<IActionResult> GetAllOrdersOfStationContainTransfer(
        [FromQuery] string? keyword,
        [FromQuery] Guid? stationId,
        [FromQuery] Guid? transferId,
        [FromQuery] Guid? collectedHubId,
        [FromQuery] string? customerStatus,
        [FromQuery] string? deliveryStatus,
        [FromQuery] string? orderBy = "CreatedAt",
        [FromQuery] bool? isAscending = false,
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

        if (defineUser.Payload.Role == "Admin")
        {
            var result = await _orderService.GetAllOrdersOfStaff(
                isAscending: isAscending,
                orderBy: orderBy,
                filter: x => (!stationId.HasValue || x.StationId == stationId) &&
                             (!collectedHubId.HasValue || x.CollectedHubId == collectedHubId) &&
                             (!transferId.HasValue || x.TransferId == transferId) &&
                             (string.IsNullOrEmpty(customerStatus) || x.CustomerStatus!.Contains(customerStatus)) &&
                             (string.IsNullOrEmpty(deliveryStatus) || x.DeliveryStatus!.Contains(deliveryStatus)),
                pageIndex: pageIndex,
                pageSize: pageSize);
            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
        }
        else
        {
            var result = await _orderService.GetAllOrdersOfStaff(
                isAscending: isAscending,
                orderBy: orderBy,
                filter: x => (x.StationId.ToString() == defineUser.Payload.AuthorizationDecision) &&
                             (!collectedHubId.HasValue || x.CollectedHubId == collectedHubId) &&
                             (!transferId.HasValue || x.TransferId == transferId) &&
                             (x.TransferId != null) &&
                             (string.IsNullOrEmpty(customerStatus) || x.CustomerStatus!.Contains(customerStatus)) &&
                             (string.IsNullOrEmpty(deliveryStatus) || x.DeliveryStatus!.Contains(deliveryStatus)),
                pageIndex: pageIndex,
                pageSize: pageSize);
            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
        }
    }


    [HttpGet]
    [Route("orders/{transferId}")]
    [Authorize(Roles = "CollectedStaff, StationStaff, Admin")]
    [SwaggerOperation(Summary = "Get all orders of a transfer - CollectedStaff, StationStaff, Admin Role - Tien")]
    public async Task<IActionResult> GetAllOrdersOfATransfer(
        [FromRoute] Guid transferId,
        [FromQuery] string? keyword,
        [FromQuery] Guid? stationId,
        [FromQuery] Guid? collectedHubId,
        [FromQuery] string? customerStatus,
        [FromQuery] string? deliveryStatus,
        [FromQuery] string? orderBy,
        [FromQuery] bool isAscending,
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

        if (defineUser.Payload.Role == "Admin")
        {
            var result = await _orderService.GetAllOrdersOfStaff(
                isAscending: isAscending,
                orderBy: orderBy,
                filter: x => x.TransferId == transferId &&
                             (!stationId.HasValue || x.StationId == stationId) &&
                             (!collectedHubId.HasValue || x.CollectedHubId == collectedHubId) &&
                             (string.IsNullOrEmpty(customerStatus) || x.CustomerStatus!.Contains(customerStatus)) &&
                             (string.IsNullOrEmpty(deliveryStatus) || x.DeliveryStatus!.Contains(deliveryStatus)),
                pageIndex: pageIndex,
                pageSize: pageSize);
            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
        }
        else if (defineUser.Payload.Role == EnumConstants.RoleEnumString.COLLECTEDSTAFF)
        {
            var result = await _orderService.GetAllOrdersOfStaff(
                isAscending: isAscending,
                orderBy: orderBy,
                filter: x => x.TransferId == transferId &&
                             (x.CollectedHubId.ToString() == defineUser.Payload.AuthorizationDecision) &&
                             (!stationId.HasValue || x.StationId == stationId) &&
                             (string.IsNullOrEmpty(customerStatus) || x.CustomerStatus!.Contains(customerStatus)) &&
                             (string.IsNullOrEmpty(deliveryStatus) || x.DeliveryStatus!.Contains(deliveryStatus)),
                pageIndex: pageIndex,
                pageSize: pageSize);
            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
        }
        else
        {
            var result = await _orderService.GetAllOrdersOfStaff(
                isAscending: isAscending,
                orderBy: orderBy,
                filter: x => x.TransferId == transferId &&
                             (x.StationId.ToString() == defineUser.Payload.AuthorizationDecision) &&
                             (!collectedHubId.HasValue || x.CollectedHubId == collectedHubId) &&
                             (string.IsNullOrEmpty(customerStatus) || x.CustomerStatus!.Contains(customerStatus)) &&
                             (string.IsNullOrEmpty(deliveryStatus) || x.DeliveryStatus!.Contains(deliveryStatus)),
                pageIndex: pageIndex,
                pageSize: pageSize);
            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
        }

        return Unauthorized("You are not allowed to view this transfer's orders");
    }


    [HttpPut]
    [Route("station/orders/update-status")]
    [Authorize(Roles = "StationStaff, Admin")]
    [SwaggerOperation(Summary = "Update order status - StationStaff, Admin Role - Tien")]
    public async Task<IActionResult> UpdateOrderStatusByStationStaff(
        [FromBody] UpdateOrderStatus.UpdateOrderStatusByTransfer request)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }

        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);


        if (defineUser.Payload.AuthorizationDecision != request.StationId.ToString())
        {
            return Unauthorized("You are not allowed to update this order status");
        }

        var result =
            await _orderService.UpdateOrderStatusByStationStaff(request, defineUser.Payload);
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
    }
}