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

    [HttpPost("orders/check-exist-cart")]
    [SwaggerOperation(Summary = "Kiểm tra đã tồn tại giỏ hàng được tạo ra cùng ngày với cùng 1 farmHubId và isPaid = false hay chưa ")]
    public async Task<IActionResult> CheckExistCart([FromBody] AddToCartRequest.CheckExistCartRequest request)
    {
        /*string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }
        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);*/

        var response = await _orderService.CheckExistCart(request.CustomerId, request.FarmHubId, request.ProductItemId, request.StationId, request.BusinessDayId, request.IsPaid);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
    
    
    [HttpPost("order/add-to-cart")]
    [SwaggerOperation(Summary = "Thêm sản phẩm vào giỏ hoặc cập nhật sản phẩm trong giỏ hàng")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        /*tring authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }
        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);*/

        var response = await _orderService.UpsertToCart(request.CustomerId, request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }
}