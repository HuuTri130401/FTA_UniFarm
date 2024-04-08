using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.UniFarm.API.Controllers;

public class AdminDashboardController : BaseController
{
    private readonly IAdminDashboardService _adminDashboardService;
    
    public AdminDashboardController(IAdminDashboardService adminDashboardService)
    {
        _adminDashboardService = adminDashboardService;
    }
    
    [HttpGet("admin/dashboard/order-statistic-by-business-day")]
    public async Task<IActionResult> GetOrderStatisticByBusinessDay([FromQuery] Guid businessDayId)
    {
        var response = await _adminDashboardService.GetOrderStatisticByBusinessDay(businessDayId, null);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}