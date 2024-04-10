using Capstone.UniFarm.Domain.Enum;
using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerOperation(Summary = "Get statistic by month- Tien")]
    public async Task<IActionResult> GetStatisticByMonth()
    {
        var result = await _adminDashboardService.GetStatisticByMonth();
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
    }
    
    [HttpGet("admin/dashboard/product-statistic")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Get product selling percent - Tien")]
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
    
    [HttpGet("admin/dashboard/farmhub-ranking")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Get top farm hub - Tien")]
    public async Task<IActionResult> GetTopFarmHub(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? top = 5)
    {
        if (fromDate == null || toDate == null)
        {
            fromDate = DateTime.Now.AddMonths(-1);
            toDate = DateTime.Now;
        }
        var response = await _adminDashboardService.GetTopFarmHub(fromDate, toDate, top);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    
    [HttpGet("admin/dashboard/balance-fluctuations")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Biến động số dư theo tháng - Tien")]
    public async Task<IActionResult> GetBalanceFluctuations()
    {
        var response = await _adminDashboardService.GetBalanceFluctuations();
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    

    [HttpGet("admin/dashboard/business-day/{businessDayId}/order-statistic")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Get order statistic by business day - Tien")]
    public async Task<IActionResult> GetOrderStatisticByBusinessDay(Guid businessDayId)
    {
        var response = await _adminDashboardService.GetOrderStatisticByBusinessDay(businessDayId, null);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    

    [HttpGet("admin/dashboard/business-day/{businessDayId}/orders")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Get all orders of business day - Tien")]
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
    [SwaggerOperation(Summary = "Get order detail - Tien")]
    public async Task<IActionResult> GetOrderDetail(Guid businessDayId, Guid orderId)
    {
        var result = await _adminDashboardService.GetOrderDetail(businessDayId, orderId);
        return result.IsError ? HandleErrorResponse(result.Errors) : Ok(result);
    }
}