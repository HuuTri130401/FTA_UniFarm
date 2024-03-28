using Capstone.UniFarm.Services.ICustomServices;
using Capstone.UniFarm.Services.ViewModels.ModelRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Capstone.UniFarm.API.Controllers;

public class CartController : BaseController
{
    private readonly ICartService _cartService;
    private readonly IAccountService _accountService;

    public CartController(ICartService cartService, IAccountService accountService)
    {
        _cartService = cartService;
        _accountService = accountService;
    }

    /*[HttpPost("carts/check-exist-cart")]
    [SwaggerOperation(Summary =
        "Kiểm tra đã tồn tại giỏ hàng được tạo ra cùng ngày với cùng 1 farmHubId và isPaid = false hay chưa ")]
    public async Task<IActionResult> CheckExistCart([FromBody] AddToCartRequest.CheckExistCartRequest request)
    {
        var response = await _cartService.CheckExistCart(request.CustomerId, request.FarmHubId, request.ProductItemId,
            request.StationId, request.BusinessDayId, request.IsPaid);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    [HttpPost("cart/add-to-cart")]
    [SwaggerOperation(Summary = "Thêm sản phẩm vào giỏ hoặc cập nhật sản phẩm trong giỏ hàng")]
    /*
    [Authorize(Roles = "Customer")]
    #1#
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        /*string authHeader = HttpContext.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
        {
            return Unauthorized();
        }
        string token = authHeader.Replace("Bearer ", "");
        var defineUser = _accountService.GetIdAndRoleFromToken(token);
        if (defineUser.Payload == null) return HandleErrorResponse(defineUser!.Errors);#1#
        var response = await _cartService.UpsertToCart(request.CustomerId, request);
        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }


    [HttpGet("carts")]
    [SwaggerOperation(Summary = "Lấy thông tin giỏ hàng của khách hàng")]
    /*
    [Authorize(Roles = "Customer")]
    #1#
    public async Task<IActionResult> GetCart(
        [FromQuery] string? searchWord,
        [FromQuery] string? orderBy,
        [FromQuery] bool? isDesc,
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

        var response = await _cartService.GetCart(filter: x =>
                x.CustomerId == defineUser.Payload.Id
                && x.IsPaid == false,
            orderBy: orderBy,
            isDesc: isDesc,
            pageIndex: pageIndex,
            pageSize: pageSize);

        return response.IsError ? HandleErrorResponse(response.Errors) : Ok(response);
    }*/
}