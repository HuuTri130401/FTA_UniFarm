using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers;

public class AdminDashboardController : BaseController
{
    private readonly IAdminDashboardService _adminDashboardService;

    public AdminDashboardController(IAdminDashboardService adminDashboardService)
    {
        _adminDashboardService = adminDashboardService;
    }

    [HttpGet("admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStatisticByMonth()
    {
        var result = await _adminDashboardService.GetStatisticByMonth();
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
    }
    
    [HttpGet("admin/dashboard/product-statistic")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetProductSellingPercent(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        if (fromDate == null || toDate == null)
        {
            fromDate = DateTime.Now.AddMonths(-1);
            toDate = DateTime.Now;
        }
        var response = await _adminDashboardService.GetProductSellingPercent(fromDate, toDate);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }

    [HttpGet("admin/dashboard/business-day/{businessDayId}/order-statistic")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrderStatisticByBusinessDay(Guid businessDayId)
    {
        var response = await _adminDashboardService.GetOrderStatisticByBusinessDay(businessDayId, null);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    

    [HttpGet("admin/dashboard/business-day/{businessDayId}/orders")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllOrdersOfBusinessDay(
        Guid businessDayId,
        [FromQuery] Guid? stationId,
        [FromQuery] Guid? transferId,
        [FromQuery] Guid? collectedHubId,
        [FromQuery] string? customerStatus,
        [FromQuery] string? deliveryStatus,
        [FromQuery] EnumConstants.FilterOrder? orderBy,
        [FromQuery] bool isAscending = false,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10
    )
    {
        var result = await _adminDashboardService.GetAllOrdersOfStaff(
            isAscending: isAscending,
            orderBy: orderBy!.Value.ToString() ?? "CreatedAt",
            filter: x => x.BusinessDayId == businessDayId &&
                         (!stationId.HasValue || x.StationId == stationId) &&
                         (!collectedHubId.HasValue || x.CollectedHubId == collectedHubId) &&
                         (!transferId.HasValue || x.TransferId == transferId) &&
                         (string.IsNullOrEmpty(customerStatus) || x.CustomerStatus!.Contains(customerStatus)) &&
                         (string.IsNullOrEmpty(deliveryStatus) || x.DeliveryStatus!.Contains(deliveryStatus)),
            pageIndex: pageIndex,
            pageSize: pageSize);
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
    }

    [HttpGet("admin/dashboard/business-day/{businessDayId}/orders/{orderId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOrderDetail(Guid businessDayId, Guid orderId)
    {
        var result = await _adminDashboardService.GetOrderDetail(businessDayId, orderId);
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
    }
}