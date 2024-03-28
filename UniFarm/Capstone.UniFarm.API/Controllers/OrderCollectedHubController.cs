using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

[Authorize(Roles = "CollectedStaff, Admin")]
public class OrderCollectedHubController : BaseController
{
    private readonly IOrderService _orderService;
    private readonly IAccountService _accountService;

    public OrderCollectedHubController(IOrderService orderService, IAccountService accountService)
    {
        _orderService = orderService;
        _accountService = accountService;
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
            var result = await _orderService.GetAllOrdersOfACollectedHub(
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
            var result = await _orderService.GetAllOrdersOfACollectedHub(
                isAscending: isAscending,
                orderBy: orderBy,
                filter: x => (x.CollectedHubId.ToString() == defineUser.Payload.AuthorizationDecision) &&
                             (!stationId.HasValue || x.StationId == stationId) &&
                             (!transferId.HasValue || x.TransferId == transferId) &&
                             (string.IsNullOrEmpty(customerStatus) || x.CustomerStatus!.Contains(customerStatus)) &&
                             (string.IsNullOrEmpty(deliveryStatus) || x.DeliveryStatus!.Contains(deliveryStatus)),
                pageIndex: pageIndex,
                pageSize: pageSize);
            return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
        }
    }
}