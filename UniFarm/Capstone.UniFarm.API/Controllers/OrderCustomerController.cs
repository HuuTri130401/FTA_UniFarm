using Capstone.UniFarm.Services.ICustomServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost("orders/check-exist-cart")]
    [Authorize]
    public async Task<IActionResult> CheckExistCart([FromQuery] Guid farmHubId, [FromQuery] Guid productItemId)
    {
        string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }
        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);

        var response = await _orderService.CheckExistCart(defineUser.Payload!.Id, farmHubId, productItemId);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}